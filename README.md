# My AI Game (合成大西瓜)

这是一个基于 Unity 引擎开发的休闲益智游戏，其核心玩法灵感来源于流行的“合成大西瓜”。玩家通过掉落和合并水果来获得更大的水果，最终目标是合成完整的大西瓜。项目不仅实现了核心玩法，还构建了一个包含城镇场景、经济系统和库存管理的扩展框架。

## ✨ 主要功能

  * **核心合成玩法**：经典的“合成大西瓜”机制，将两个相同的水果合并成一个更大的新水果。
  * **多样化的管理器**：
      * `GameManager`: 控制游戏整体状态和流程。
      * `FruitSpawner`: 负责水果的生成和掉落。
      * `ScoreManager`: 管理和计算玩家得分。
      * `UIManager`: 负责游戏内的 UI 更新和交互，如得分、游戏结束等。
      * `AudioManager`: 管理背景音乐和游戏音效。
  * **特殊道具/机制**：
      * **黑洞 (`BlackHole`)**: 游戏中的特殊元素，可能会影响场上的水果。
      * **天气系统 (`WeatherManager`)**: 动态天气效果，为游戏增添视觉趣味性。
  * **城镇与经济系统**：
      * **城镇场景 (`TownScene`)**: 一个独立于核心玩法的场景，为玩家提供交互和升级的中心。
      * **经济系统 (`EconomySystem`)**: 实现了游戏内的货币逻辑。
      * **库存系统 (`InventorySystem`)**: 允许玩家存储和管理物品。
  * **完善的场景管理**：
      * `CoreScene`: 管理核心系统和全局事件。
      * `WatermelonScene`: 核心游戏场景。
      * `TownScene`: 城镇/主菜单场景。

## 🛠️ 技术栈

  * **游戏引擎**: Unity 2022.3.5f1c1
  * **编程语言**: C\#
  * **核心插件**:
      * TextMesh Pro: 用于实现高质量的文本渲染。
      * Bytedance Stark SDK: 集成了字节跳动的 SDK，可能用于发布到抖音小游戏等平台。
      * Heureka AssetHunter PRO: 用于项目资源管理的编辑器工具。

## 🚀 如何开始

### 环境要求

  * **Unity Editor**: 版本 `2022.3.5f1c1` 或更高。
  * 安装了 **Visual Studio** 并配置好了 Unity 开发环境。

### 安装与运行

1.  **克隆仓库**
    ```bash
    git clone https://github.com/juanhugithub/my-AI-game.git
    ```
2.  **打开项目**
      * 打开 Unity Hub。
      * 点击 "打开" -\> "从磁盘添加项目"。
      * 选择克隆下来的项目文件夹。
3.  **运行游戏**
      * 在 Unity 编辑器中，打开 `Assets/_Scenes/` 目录。
      * 首先打开 `CoreScene`，然后从 `Hierarchy` 面板中运行它。或者直接打开 `TownScene` 作为游戏主入口。
      * 点击 Unity 编辑器顶部的 "Play" 按钮即可开始游戏。

## 📂 项目结构

```
my-AI-game/
├── Assets/
│   ├── _Scenes/         # 游戏场景
│   ├── _Scripts/        # 核心 C# 脚本
│   │   ├── Core/        # 核心系统 (场景加载, 数据管理等)
│   │   ├── Data/        # 数据结构 (玩家数据, 物品数据)
│   │   ├── Gameplay/    # 核心玩法逻辑
│   │   │   └── _Watermelon/ # “合成大西瓜”玩法的具体实现
│   │   └── UI/          # UI 管理脚本
│   ├── _Prefabs/        # 预制体
│   ├── HCDXG/           # 游戏美术、音效等资源
│   └── Resources/       # 资源文件 (道具等)
├── ProjectSettings/     # Unity 项目配置
└── Packages/            # 包管理器清单
```

## 🤝 贡献

欢迎对该项目进行 Fork 和提交 Pull Request。如果您发现了 bug 或者有好的建议，请随时提出 Issue。

-----

**使用说明:**

  * 将以上文本内容（从 `# My AI Game` 开始）复制到一个名为 `README.md` 的文本文件中。
  * 将这个 `README.md` 文件上传到您 GitHub 仓库的根目录。
  * `![游戏截图]` 部分的链接是您之前提供的截图，您可以根据需要替换成更具代表性的游戏截图。

希望这份 README 对您有帮助！
