Минимальная игра «Ферма».
Кратко: приложение «Ферма» — Vue 3 frontend + ASP.NET Core Web API (C#) + PostgreSQL.
Реализованы регистрация/логин, ферма (поля/посадка/сбор), магазин семян, инвентарь и таблица лидеров.

Используемые технологии:
Frontend: Vue 3, Pinia, Vue Router, Axios, (опционально TypeScript).
Backend: ASP.NET Core Web API (C#), EF Core (Postgres provider).
База данных: PostgreSQL.
Тесты: xUnit (backend), Vitest (frontend).
Контейнеризация: Docker Compose (postgres, backend, frontend).
Валидация: FluentValidation (рекомендация).
Логирование: Serilog (рекомендация).

Структура проекта (рекомендованная).
/backend — ASP.NET Core проект;
/Controllers;
/Services;
/Data (DbContext, миграции);
/Models/DTOs;
/Tests (xUnit);
/frontend — Vue 3 проект;
/src (views, components, stores);
/tests (Vitest);
docker-compose.yml;
.env.example;
README.md.

Как запустить локально (без Docker):
Установите .NET 7+ SDK, Node.js 18+, Postgres.
Настройте Postgres и обновите connection string в backend/appsettings.Development.json.
В каталоге backend выполните:
dotnet ef database update,
dotnet run,
В каталоге frontend выполните:
npm install,
npm run dev.

Как запустить через Docker Compose:
Скопируйте .env.example → .env и заполните значения.
Выполните:
docker-compose up --build.
Backend будет доступен на http://localhost:5000/ (настройки в docker-compose).

Миграции и инициализация данных:
Миграции создаются через EF Core: dotnet ef migrations add Initial.
Пример начальных seed-данных (в миграции/DbContext seeder):
три вида культур (Carrot, Potato, Strawberry) с разными ценами и временем созревания.
при регистрации пользователю даётся стартовый баланс и 2 стартовых поля.

Авторизация:
Используется JWT.
Пароли хранятся в БД как безопасные хеши (ASP.NET Identity PasswordHasher или PBKDF2/Argon2).
Приватные endpoint защищены атрибутом [Authorize].

Основные API (кратко):
POST /api/auth/register — регистрация;
POST /api/auth/login — логин (возвращает JWT);
GET /api/shop/seeds — список семян;
POST /api/shop/buy-seed — покупка семян;
GET /api/farm — данные фермы, полей, баланса;
POST /api/fields/{id}/plant — посадить семена;
POST /api/fields/{id}/harvest — собрать урожай;
GET /api/leaderboard — таблица лидеров (public);
GET /api/inventory — инвентарь;
POST /api/inventory/sell — продать урожай.

Бизнес-правила (ключевые):
Время созревания считается на сервере; клиент не может завершить выращивание сам.
Все денежные операции выполняются в транзакциях БД.
Запрещено: сажать на занятое поле, собирать до времени, покупать без баланса, двойной сбор.

Тесты:
Backend: dotnet test ./backend/Tests.
Frontend: npm run test в папке frontend.
Минимальный набор тестов включён в проект (пример ниже).
Что можно улучшить (при доп. времени):
Refresh-Token flow для JWT.
WebSocket/Server-Sent Events для realtime обновления полей и таблицы лидеров.
Argon2 для хеширования паролей.
Более сложная экономика и marketplace между игроками.
Больше тестов интеграции с реальным Postgres в CI.

Известные ограничения:
Реалтайм обновлений нет (по умолчанию — polling).
Упрощённая экономика без микротранзакций.
Минимальные проверки rate-limiting.

Запуск тестов (локально):
Backend:
Перейти в backend → dotnet test.
Frontend:
Перейти в frontend → npm ci && npm run test.

Контакт/дополнительно:
Все решения и допущения зафиксированы в README.
В вопросах по развертыванию — можно прислать содержимое .env (без секретов) для подсказок по конфигу.