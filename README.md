# HeroDemo · 2D 横版冒险游戏

一个用 **Unity 2022.3 + URP 2D** 制作的 2D 横版冒险小项目，用于学习 Unity 的 2D 物理、Input System、Animator 状态机与 C# 脚本结构。

包含玩家**三段连招**、受击硬直与击退，以及会**巡逻、撞墙转身、受击反击**的野猪敌人。代码全部手写，未使用任何付费插件。

---

## 特性

### 玩家
- Input System 驱动：移动 / 跳跃 / 攻击
- **三段连招**：`comboStep` 整数参数 + Animator Any State 过渡，超过连招窗口自动重置
- 受击反馈：击退 + 受伤动画 + 无敌帧

### 敌人（野猪）
- 巡逻移动（撞墙 → 停顿等待 → 转身）
- 受击：转身面向攻击者、硬直、被击退
- Animator 状态机：`BoarIdle` / `BoarWalk` / `BoarRun` / `BoarHurt` / `BoarDead`

### 通用
- **`Attack` 攻击判定**：LayerMask 过滤 + 每次挥砍对同一目标只结算一次
  （同时实现 `OnTriggerEnter2D` 与 `OnTriggerStay2D`，站在敌人身上连击也能正常命中）
- **`Character` 通用血量组件**：血量、受伤、无敌帧、死亡，通过 `UnityEvent` 通知动画与控制器
- **`PhysicsCheck` 检测组件**：地面 / 左右墙检测；`manual` 关闭时按碰撞体尺寸**自动**计算检测点

---

## 操作

| 操作 | 按键 |
| --- | --- |
| 移动 | `A` / `D` 或 `←` / `→` |
| 跳跃 | `空格` |
| 攻击（可连按出三段） | `J` |

> 手柄（左摇杆 / 十字键）的绑定也在 `PlayerInputControl` 中，可直接使用。

---

## 环境要求

| 项 | 说明 |
| --- | --- |
| Unity | **2022.3.62f3c1**（中国版）<br>全球版用 `2022.3.x` 打开即可，Unity 会提示版本不一致并自动迁移 |
| 渲染管线 | URP 14.0.12（随工程自动获取） |
| 输入 | Input System 1.14.2（工程已设为「仅新输入系统」） |
| 其他 | TextMeshPro / Timeline / Visual Scripting（Unity 官方包） |

无需任何额外插件或付费资源。

---

## 如何运行

```bash
git clone https://github.com/john-aston/HeroDemo-Unity.git
```

1. 打开 **Unity Hub** → `Add` → `Add project from disk` → 选择项目根目录
2. 使用 **Unity 2022.3.62f3c1**（或任意 2022.3.x）打开
3. **首次打开会比较慢**：`Library/` 不在仓库中，Unity 需要重新导入约 1200 个资源文件并编译脚本
4. 打开场景 `Assets/Scenes/SampleScene.unity`
5. 点击 **Play**

### 常见问题

| 现象 | 原因 / 处理 |
| --- | --- |
| 打开后场景是空的 | 双击 `Assets/Scenes/SampleScene.unity` |
| 提示 "Package resolution failed" | 工程默认使用中国镜像 `packages.unity.cn`（见 `Project Settings → Package Manager`），海外网络可能很慢，可临时切换到 `packages.unity.com` |
| 提示 Unity 版本不一致 | 正常现象，选择继续即可；会改写 `ProjectSettings/ProjectVersion.txt` |
| 角色不动 / 没有输入 | 确认 `Project Settings → Player → Active Input Handling` 为 `Input System Package (New)` |

---

## 项目结构

```text
Assets/
├── Scenes/                     游戏场景
│   └── SampleScene.unity
├── scripts/
│   ├── General/                通用组件
│   │   ├── Character.cs        血量 / 受伤 / 无敌 / 死亡
│   │   ├── Attack.cs           攻击判定框
│   │   └── PhysicsCheck.cs     地面与墙体检测
│   ├── player/                 玩家
│   │   ├── PlayerController.cs 输入、移动、跳跃、连招计数
│   │   ├── PlayerAnimation.cs  向 Animator 写入参数
│   │   └── HurtAnimation.cs    StateMachineBehaviour：受伤结束复位
│   └── Enemy/                  敌人
│       ├── Enemy.cs            敌人基类：巡逻 / 撞墙转身 / 受击硬直
│       └── Boar.cs             野猪（继承 Enemy）
├── animations/
│   ├── player/                 玩家动画剪辑 + Player.controller
│   └── Enemy/Boar/             野猪动画剪辑 + Boar.controller
├── Art Assets/                 美术素材（第三方开源素材）
└── Settings/                   URP / Input System 配置
```

---

## 核心脚本说明

| 脚本 | 关键点 |
| --- | --- |
| `PlayerController` | `comboStep` 记录当前段数，`comboWindow` 控制连招间隔；每次按键清空攻击框的命中记录 |
| `PlayerAnimation` | `SetFloat("velocityX")` 等写入 Animator；攻击时先 `SetInteger("comboStep")` 再 `SetTrigger("attack")` |
| `Character` | `TakeDamage()` 扣血并触发 `OnTakeDamage` / `OnDie` 两个 `UnityEvent` |
| `Attack` | `TryHit()` 做 LayerMask 过滤 + 命中记录，保证一刀只打一次 |
| `PhysicsCheck` | `Physics2D.OverlapCircle` 检测地面与两侧墙体；`manual` 关闭时用 `collider.bounds` 自动布点 |
| `Enemy` | `wait` 负责撞墙停顿，`isHurt` 期间不接管速度（让击退冲量生效），协程控制硬直时长 |
| `AttackFinish` / `HurtAnimation` | StateMachineBehaviour，在动画状态结束时复位 `isAttack` / `isHurt` |

---

## 已知限制 / 待办

- `BoarDead` 状态已接入 Animator，但尚未由代码触发（`dead` 参数还没有设置逻辑）
- 场景中保留了少量预留资源（例如当前未被引用的 `Player.controller`），后续整理
- 尚未实现：相机跟随、敌人追击（`chaseSpeed` 已预留）、关卡切换、音效

---

## 第三方资源

| 资源 | 用途 |
| --- | --- |
| **Legacy-Fantasy-High Forest** | 地图瓦片、背景、敌人素材 |
| **Generic-Character** | 玩家角色素材 |

均为**开源 / 免费** 2D 素材包。素材版权归原作者所有，本项目按其原始授权使用；
如需在其它项目中使用，请遵循原素材的授权条款。

> 建议在此补充原素材的来源链接，方便他人追溯。

---

## 授权

- **代码**：MIT License，详见 [LICENSE](LICENSE)
- **美术素材**：版权归原作者，见上方「第三方资源」

---

## 关于

个人学习项目，欢迎 issue 交流。如果对你有帮助，给个 Star 就是最大的鼓励。
