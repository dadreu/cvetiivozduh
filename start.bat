@echo off
echo ==============================================
echo Запуск проекта "Цветы и Воздух" (Локально)
echo ==============================================

echo Запуск Backend (C# Kestrel)...
start "Backend" cmd /c "cd Backend && dotnet run"

echo Запуск Frontend (Vite)...
start "Frontend" cmd /c "cd Frontend && npm run dev"

echo.
echo Проект запускается. Откройте браузер по адресу: http://localhost:5173
echo.
pause
