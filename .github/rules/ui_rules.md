# 前端、UI设计系统规范

## 色彩系统
- CSS变量：`--color-primary`、`--color-secondary`、`--color-neutral-*`、`--color-success`、`--color-warning`、`--color-error`
- 背景色：仅限 `#ffffff` 或 `#f8f9fa`、`#f3f4f6`
- **禁止**：背景/按钮渐变、蓝紫色渐变、霓虹色、彩虹色系
- 品牌颜色：单视图不超过3种
- 文本颜色：`#111827`（主）、`#6b7280`（次）、`#9ca3af`（三）

## 排版
- 字体大小：12px / 14px / 16px / 20px / 24px / 32px
- CSS变量：`--text-xs` / `--text-sm` / `--text-base` / `--text-lg` / `--text-xl` / `--text-2xl`
- 正文：font-weight 400，line-height 1.5
- 标题：font-weight 600，line-height 1.25
- 单位系统统一，禁止混用 px/rem/em
- 禁止使用定义范围外的任意字体尺寸

## 间距
- 4px 基础网格：4 / 8 / 12 / 16 / 24 / 32 / 48 / 64px
- CSS变量：`--space-1` 到 `--space-16`
- 禁止魔法数字（13px、7px、23px 等）
- 组件族内填充一致

## 组件规范

### 卡片
- 边框 **或** 阴影，二选一
- 阴影级别1：`0 1px 3px rgba(0,0,0,0.08)`
- 阴影级别2：`0 4px 12px rgba(0,0,0,0.1)`
- 圆角：6px 或 8px，禁止 16px+

### 按钮
- 主按钮：实心填充，无渐变
- 次按钮：轮廓或幽灵样式
- 悬停：变暗 10%，不切换颜色
- 禁止圆角矩形按钮使用 `rounded-full`

### 输入框
- 边框：`1px solid #d1d5db`
- 圆角：6px
- 焦点：边框颜色变化 + outline，禁止 glow 效果

## 图标
- 统一使用一个图标集：Lucide / Heroicons / Phosphor
- 尺寸：16px（内联）、20px（独立）
- 禁止使用 emoji 作为功能图标

## 禁止事项
- 蓝紫色渐变
- 玻璃态效果（除非明确要求）
- emoji 图标
- 过多阴影
- 颜色/间距/排版的内联样式
- 魔法数字（每个值必须引用设计令牌）
- 单页超过 2 个阴影深度级别