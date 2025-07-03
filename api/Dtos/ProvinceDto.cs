namespace api.Dtos
{
public class ProvinceDto
{
	public int code { get; set; }
	public string? name { get; set; }
	public List<DistrictDto>? districts { get; set; }
}
public class DistrictDto
{
	public int code { get; set; }
	public string? name { get; set; }
	public List<WardDto>? wards { get; set; }
}
public class WardDto
{
	public int code { get; set; }
	public string? name { get; set; }

}
}
