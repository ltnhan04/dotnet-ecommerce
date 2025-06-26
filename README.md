# 🌳 **Git Flow**

Git Flow introduces a robust branching model for project development. Below is the structure and workflow for managing branches like `main`, `develop`, `release`, and `hotfix`.

## 7.1. **Branching Model Overview**

![Git Flow Model](https://media.licdn.com/dms/image/v2/D4E22AQFv9-n24c-x0A/feedshare-shrink_800/feedshare-shrink_800/0/1708786844976?e=2147483647&v=beta&t=DZ7YCMW3f5orafrSi1CXZLEB3J_YfZaWe2Gpnb0jigI)

### 7.2. **Branch Definitions**

| **Branch**  | **Purpose**                                                                |
| ----------- | -------------------------------------------------------------------------- |
| **main**    | Contains production-ready code. Reflects what is currently live.           |
| **develop** | Holds the latest code under development. Acts as an integration branch.    |
| **feature** | Branch off `develop` to work on individual features.                       |
| **release** | Prepare code for production. Merge back into `main` and `develop`.         |
| **hotfix**  | For urgent fixes directly on `main`. Merge into both `main` and `develop`. |

### 7.3. **Workflow Steps**

#### 1. **Starting a Feature**

- Branch from `develop`:
  ```bash
  git checkout develop
  git checkout -b feature/{feature-name}
  ```

#### 2. **Finishing a Feature**

- Merge into `develop`:
  ```bash
  git checkout develop
  git merge feature/{feature-name}
  git branch -d feature/{feature-name}
  ```

#### 3. **Preparing a Release**

- Branch from `develop`:
  ```bash
  git checkout develop
  git checkout -b release/{version}
  ```
- Test and fix issues.
- Merge into `main` and tag the release:
  ```bash
  git checkout main
  git merge release/{version}
  git tag -a v{version} -m "Release {version}"
  ```
- Merge back into `develop`:
  ```bash
  git checkout develop
  git merge release/{version}
  git branch -d release/{version}
  ```

#### 4. **Hotfixing**

- Branch from `main`:
  ```bash
  git checkout main
  git checkout -b hotfix/{issue-name}
  ```
- Fix the issue and merge into both `main` and `develop`:

  ```bash
  git checkout main
  git merge hotfix/{issue-name}
  git tag -a v{hotfix-version} -m "Hotfix {issue-name}"

  git checkout develop
  git merge hotfix/{issue-name}
  git branch -d hotfix/{issue-name}
  ```
