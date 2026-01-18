<p align="center">
  <img src="Resources/app.ico" width="128" height="128" alt="MinimalPomodoro Logo">
</p>

<h1 align="center">MinimalPomodoro 极简番茄钟</h1>

<p align="center">
  <a href="https://github.com/Apauto-to-all/MinimalPomodoro/releases">
    <img src="https://img.shields.io/github/v/release/Apauto-to-all/MinimalPomodoro?color=red&label=version&logo=github" alt="Latest Release">
  </a>
  <img src="https://img.shields.io/badge/size-%3C%20300%20KB-brightgreen?logo=speedtest" alt="Size">
  <img src="https://img.shields.io/badge/platform-Windows_10%2F11-blue?logo=windows" alt="Platform">
  <img src="https://img.shields.io/badge/lang-7--Languages-orange?logo=translate" alt="Languages">
</p>

<p align="center">
  <strong>专为 Windows 设计的极简番茄钟，完全寄宿于系统托盘，通过一颗“动态番茄”管理你的专注时间。</strong>
</p>

---

## ✨ 核心特性

- **极致轻量**：安装包仅约 **300 KB**，单文件绿色运行，不占资源。
- **无界面交互**：所有操作均在托盘区完成。托盘图标会根据剩余时间**动态填充/消融**，无需点击即可一眼看清进度。
- **动态状态图标**：
  - 🍎 **红色**：专注工作中（倒计时消耗）。
  - 🍏 **绿色**：短休息（恢复中）。
  - 🔵 **蓝色**：长休息（深度恢复）。
  - ⏸️ **灰色**：已暂停。
- **音乐播放器式控制面板**：右键菜单顶部包含一个直观的控制中心，支持切歌式切换阶段，并配有胶囊进度条。
- **多国语言支持**：支持 简体中文、English、日本語、Deutsch、Español、Français、한국어，根据系统语言自动识别。
- **灵活通知**：支持阶段结束提醒及“预先提醒”功能（例如在结束前 60 秒告知）。
- **绿色开源**：配置存储在 `%APPDATA%`，退出干净，无残留。

## 🚀 快速上手

1. **运行**：双击 `MinimalPomodoro.exe`。
2. **操作**：
   - **双击图标**：快速 播放/暂停。
   - **右键图标**：打开控制面板、切换语言或进入详细设置。
3. **设置**：在设置界面中，你可以自由调整番茄时长、提醒间隔以及开机自启选项。

## 🛠️ 技术栈

- 基于 **.NET 10.0** 开发。
- 使用 **WinForms (GDI+)** 动态生成高保真状态图标。
- 采用 **Framework-dependent** 发布策略，体积优化到极致。

## 📁 目录结构说明

- `Models/`: 数据模型与常量。
- `Services/`: 计时引擎、配置管理与国际化逻辑。
- `UI/`: 托盘上下文、图标生成器及所有视觉组件。
- `Resources/`: 包含程序图标 `app.ico`。

## 📜 许可证

本项目遵循 [MIT License](LICENSE.txt)。

---

> "Stay focused, stay minimal." 🍅
