using api.Dtos;
using api.Interfaces.Repositories;
using api.models;
using api.Utils;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using api.Services;

namespace api.Repositories.Admin
{
    public class AdminOrderRepository : IAdminOrderRepository
    {
        private readonly iTribeDbContext _context;
        private readonly EmailService _sendEmail;

        public AdminOrderRepository(iTribeDbContext context, EmailService sendEmail)
        {
            _context = context;
            _sendEmail = sendEmail;
        }
        public async Task<PaginateDto<AdminGetAllOrder>> GetAllOrder(int page = 1, int size = 10)
        {
            var totalOrders = await _context.Orders.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalOrders / size);
            var orders = await _context.Orders
                .OrderByDescending(order => order._id)
                .Skip((page - 1) * size)
                .Take(size)
                .ToListAsync();

            var userId = orders.Select(item => item.user).Distinct().ToList();
            var users = await _context.Users
                .Where(item => userId.Contains(item._id))
                .ToListAsync();

            var variantId = orders.SelectMany(item => item.variants.Select(v => v.variant)).Distinct().ToList();
            var variants = await _context.ProductVariants
                .Where(item => variantId.Contains(item._id))
                .ToListAsync();

            var userMap = users.ToDictionary(user => user._id, user => user);
            var variantMap = variants.ToDictionary(variant => variant._id, variant => variant);

            var data = orders
                .Where(order => userMap.ContainsKey(order.user) && userMap.GetValueOrDefault(order.user).name != null)
                .Select(order => new AdminGetAllOrder
                {
                    _id = order._id.ToString(),
                    user = new AdminResponseUserDto
                    {
                        name = userMap.GetValueOrDefault(order.user)?.name ?? "null",
                        email = userMap.GetValueOrDefault(order.user)?.email ?? "null",
                        phoneNumber = userMap.GetValueOrDefault(order.user)?.phoneNumber ?? "null",
                        address = new AdminAddressDto
                        {
                            street = userMap.GetValueOrDefault(order.user)?.address.street ?? "null",
                            ward = userMap.GetValueOrDefault(order.user)?.address.ward ?? "null",
                            district = userMap.GetValueOrDefault(order.user)?.address.district ?? "null",
                            city = userMap.GetValueOrDefault(order.user)?.address.city ?? "null",
                            country = userMap.GetValueOrDefault(order.user)?.address.country ?? "null",
                        }
                    },
                    variants = order.variants.Select(v =>
                    {
                        return new AdminResponseVariantDto
                        {
                            color = new AdminResponseColorDto
                            {
                                colorCode = variantMap.GetValueOrDefault(v.variant).color.colorCode ?? "null",
                                colorName = variantMap.GetValueOrDefault(v.variant).color.colorName ?? "null"
                            },
                            storage = variantMap.GetValueOrDefault(v.variant)?.storage ?? "0",
                            images = variantMap.GetValueOrDefault(v.variant)?.images.FirstOrDefault()! ?? "null"
                        };
                    }).ToList(),
                    totalAmount = order.totalAmount,
                    status = order.status,
                    paymentMethod = order.paymentMethod
                }).ToList();

            return new PaginateDto<AdminGetAllOrder>
            {
                total = totalOrders,
                page = totalPages,
                currentPage = page,
                items = data
            };
        }

        public async Task<AdminGetAllOrder> GetOrderDetail(string orderId)
        {
            var order = await _context.Orders
                .FirstOrDefaultAsync(item => item._id == ObjectId.Parse(orderId));

            var users = await _context.Users.ToListAsync();
            var user = users
                .FirstOrDefault(item => item._id == order.user);

            var variantId = order.variants.Select(v => v.variant).ToList();
            var variant = await _context.ProductVariants
                .Where(item => variantId.Contains(item._id))
                .ToListAsync();

            return new AdminGetAllOrder
            {
                _id = order._id.ToString(),
                user = new AdminResponseUserDto
                {
                    name = user.name,
                    email = user.email,
                    phoneNumber = user.phoneNumber ?? "null",
                    address = new AdminAddressDto
                    {
                        street = user.address.street ?? "null",
                        ward = user.address.ward ?? "null",
                        district = user.address.district ?? "null",
                        city = user.address.city ?? "null",
                        country = user.address.country ?? "null"
                    }
                },
                variants = variant.Select(v => new AdminResponseVariantDto
                {
                    color = new AdminResponseColorDto
                    {
                        colorCode = v.color.colorCode,
                        colorName = v.color.colorName
                    },
                    storage = v.storage,
                    images = v.images.FirstOrDefault()! ?? "null",
                    quantity = v.stock_quantity
                }).ToList(),
                totalAmount = order.totalAmount,
                status = order.status,
                paymentMethod = order.paymentMethod
            };
        }

        public async Task<AdminResponseUpdateOrderStatus> UpdateOrderStatus(string orderId, stateDto dto)
        {
            var order = await _context.Orders
                .Where(item => item._id == ObjectId.Parse(orderId))
                .FirstOrDefaultAsync();

            var currentStatus = order.status;
            if (currentStatus == "delivered" || currentStatus == "cancel")
            {
                throw new AppException("Order cannot be updated from its current status", 400);
            }

            var validTransitions = new Dictionary<string, List<string>>
            {
                ["pending"] = new List<string> { "processing", "cancel" },
                ["processing"] = new List<string> { "shipped", "cancel" },
                ["shipped"] = new List<string> { "delivered" }
            };
            if (!validTransitions[currentStatus].Contains(dto.status))
            {
                throw new AppException($"Cannot change status from {currentStatus} from {dto.status}", 400);
            }

            if (currentStatus == "pending" && dto.status == "processing")
            {
                foreach (var item in order.variants)
                {
                    var productVariant = await _context.ProductVariants.ToListAsync();
                    var variant = productVariant
                        .FirstOrDefault(v => v._id == item.variant);

                    variant.stock_quantity -= item.quantity;
                    if (variant.stock_quantity < 0)
                    {
                        throw new AppException($"Insufficient stock for product");
                    }
                    await _context.SaveChangesAsync();
                }
            }

            if (dto.status == "cancel")
            {
                foreach (var item in order.variants)
                {
                    var productVariant = await _context.ProductVariants.ToListAsync();
                    var variant = productVariant
                        .FirstOrDefault(v => v._id == item.variant);

                    variant.stock_quantity += item.quantity;
                    await _context.SaveChangesAsync();
                }
            }

            if (dto.status == "delivered")
            {
                var users = await _context.Users.ToListAsync();
                var user = users
                    .FirstOrDefault(item => item._id == order.user);

                var placeholders = new Dictionary<string, string>
                {
                    {"customerName", user.name},
                    {"orderId", order._id.ToString()},
                    {"orderDate", order.createdAt.ToString("dd/MM/yyyy")},
                    {"deliveryDate", DateTime.Now.ToString("dd/MM/yyyy")},
                    {"totalAmount", order.totalAmount.ToString()},
                    {"orderItems", order.variants.ToString()!},
                    {"shippingAddress", order.shippingAddress},

                };

                await _sendEmail.SendOrderConfirmationEmail(user.email, EmailTemplates.OrderConfirmationEmail(user.name, order._id.ToString(), order.createdAt.ToString(), new DateTime().ToString(), order.totalAmount.ToString(), order.variants.ToString()!, order.shippingAddress.ToString()), placeholders);
            }
            order.status = dto.status;
            await _context.SaveChangesAsync();

            return new AdminResponseUpdateOrderStatus
            {
                _id = order._id.ToString(),
                user = order.user.ToString(),
                variants = order.variants.Select(v =>
                {
                    return new AdminOrderUpdated
                    {
                        variant = v.variant.ToString(),
                        quantity = v.quantity
                    };
                }).ToList(),
                totalAmount = order.totalAmount,
                shippingAddress = order.shippingAddress,
                status = order.status,
                paymentMethod = order.paymentMethod
            };
        }
    }
}