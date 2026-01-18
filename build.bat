@echo off
chcp 65001 > nul
setlocal

:: 设置变量
set PROJECT_NAME=MinimalPomodoro
set RELEASE_DIR=releases
set PUBLISH_PATH=bin\Release\net10.0-windows\win-x64\publish

:: 获取版本号 (从 .csproj 中提取)
set VERSION=
for /f "tokens=2 delims=<> " %%a in ('findstr "<Version>" %PROJECT_NAME%.csproj 2^>nul') do (
    set VERSION=%%a
)
if "%VERSION%"=="" (
    echo [警告] 无法从 .csproj 提取版本号，将默认使用 1.0.0
    set VERSION=1.0.0
)

set EXE_NAME=%PROJECT_NAME%_v%VERSION%_win-x64.exe

echo [1/3] 正在清理并强制结束可能运行的进程...
taskkill /f /im %PROJECT_NAME%.exe >nul 2>&1
if exist "bin" rd /s /q "bin"
if exist "obj" rd /s /q "obj"

echo [2/3] 正在编译 %PROJECT_NAME% v%VERSION% (依赖框架模式 - 极简体积)...
:: 使用更简洁且有效的参数，确保不包含运行时
dotnet publish -c Release -p:SelfContained=false -p:PublishSingleFile=true -p:DebugType=None -p:DebugSymbols=false

if %ERRORLEVEL% neq 0 (
    echo.
    echo 错误：编译失败！
    pause
    exit /b %ERRORLEVEL%
)

:: 设置发布路径 (dotnet 在 windows 上默认会生成到 win-x64 或兼容目录)
set PUBLISH_PATH=bin\Release\net10.0-windows\win-x64\publish
if not exist "%PUBLISH_PATH%" set PUBLISH_PATH=bin\Release\net10.0-windows\publish
if not exist "%RELEASE_DIR%" mkdir "%RELEASE_DIR%"

:: 移动并重命名
copy /y "%PUBLISH_PATH%\%PROJECT_NAME%.exe" "%RELEASE_DIR%\%EXE_NAME%" > nul

echo.
echo ========================================================
echo 编译成功！
echo 发布文件位于：%RELEASE_DIR%\%EXE_NAME%
for %%I in ("%RELEASE_DIR%\%EXE_NAME%") do echo 文件大小：%%~zI 字节
echo ========================================================
echo.
