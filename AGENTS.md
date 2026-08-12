# Codex 协作说明

## 项目入口

- 主解决方案：`AutoCrosstalk.sln`
- 串扰工具：`AutoCrosstalk/AutoCrosstalk.csproj`
- 协议服务：`SimpleProtocolServer/SimpleProtocolServer.csproj`
- 冒烟检查：`AutoCrosstalk.SmokeTests` 与 `SimpleProtocolServer.SmokeTests`
- 构建：`dotnet build AutoCrosstalk.sln -c Release`

## 修改约定

- `main` 保持可构建；日常修改默认使用 `codex/<任务名>` 分支。
- `.00备份` 是历史副本，不修改、不提交；当前源码以仓库根目录为准。
- 独立的 `D:/CHATGPT_file/报文定时发送` 是 SimpleProtocolServer 的权威项目；若这里的派生副本也需变化，要明确同步策略。
- 不提交构建物、用户配置、密钥、日志、截图或渲染检查产物。
- 提交前运行 Release 构建和相关 SmokeTests；硬件、网络、屏幕投影验证范围需在交付中说明。

