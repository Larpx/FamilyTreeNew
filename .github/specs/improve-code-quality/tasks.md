# 代码质量提升与Nuget升级 - 实现计划

## [ ] Task 1: 升级所有Nuget包到最新稳定版
- **Priority**: P0
- **Depends On**: None
- **Description**: 
  - 升级所有项目中的Nuget包到最新稳定版本
  - 确保版本兼容性，保持net10.0目标框架
- **Acceptance Criteria Addressed**: Nuget包升级
- **Test Requirements**:
  - `programmatic`: 运行dotnet restore成功
  - `human-judgement`: 确认所有包版本为最新稳定版

## [x] Task 2: 修复Nuget升级带来的编译错误和警告
- **Priority**: P0
- **Depends On**: Task 1
- **Description**: 
  - 解决Nuget升级后API变更导致的编译错误
  - 消除过时API警告
  - 确保代码符合Linux Docker环境要求
- **Acceptance Criteria Addressed**: 编译错误修复
- **Test Requirements**:
  - `programmatic`: 运行dotnet build成功且无警告
  - `programmatic`: Docker构建成功

## [x] Task 3: 为Models项目添加代码注释
- **Priority**: P1
- **Depends On**: None
- **Description**: 
  - 为FamilyTreeNew.Models项目中的所有实体类、DTO类添加XML注释
  - 包括实体类、DTO类、辅助类
- **Acceptance Criteria Addressed**: 代码注释完善
- **Test Requirements**:
  - `human-judgement`: 检查所有公共成员是否有注释

## [ ] Task 4: 为DAL项目添加代码注释
- **Priority**: P1
- **Depends On**: None
- **Description**: 
  - 为FamilyTreeNew.DAL项目中的所有仓库类、上下文类添加XML注释
- **Acceptance Criteria Addressed**: 代码注释完善
- **Test Requirements**:
  - `human-judgement`: 检查所有公共成员是否有注释

## [ ] Task 5: 为BLL项目添加代码注释
- **Priority**: P1
- **Depends On**: None
- **Description**: 
  - 为FamilyTreeNew.BLL项目中的所有服务类、辅助类添加XML注释
- **Acceptance Criteria Addressed**: 代码注释完善
- **Test Requirements**:
  - `human-judgement`: 检查所有公共成员是否有注释

## [ ] Task 6: 为Api项目添加代码注释
- **Priority**: P1
- **Depends On**: None
- **Description**: 
  - 为FamilyTreeNew.Api项目中的所有控制器、中间件添加XML注释
- **Acceptance Criteria Addressed**: 代码注释完善
- **Test Requirements**:
  - `human-judgement`: 检查所有公共成员是否有注释

## [ ] Task 7: 完善单元测试覆盖
- **Priority**: P1
- **Depends On**: Task 2
- **Description**: 
  - 为现有测试项目添加更多单元测试
  - 覆盖核心业务逻辑和边界条件
- **Acceptance Criteria Addressed**: 单元测试完善
- **Test Requirements**:
  - `programmatic`: 所有测试通过
  - `human-judgement`: 测试覆盖主要代码路径

# Task Dependencies
- Task 2 depends on Task 1
- Task 7 depends on Task 2
