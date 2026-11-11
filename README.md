# Ticket Notifications API

ASP.NET Core Web API (.NET 8): створення тікета запускає сповіщення через канали Email, SMS і Push.

## Запуск

```
dotnet run --project TicketNotifications.Api
```

Swagger: http://localhost:5105/swagger

## Тести

```
dotnet test
```

## Ендпоінти

- `POST /api/tickets` створює тікет і 3 сповіщення зі статусом Pending
- `POST /api/tickets/{id}/notify` надсилає всі Pending/Failed сповіщення
- `GET /api/tickets/{id}` повертає тікет зі сповіщеннями

## Архітектура

- `INotificationChannel` один інтерфейс, три реалізації (Email, SMS, Push) зі своїми правилами валідації. Новий канал це новий клас і один рядок реєстрації в `Program.cs`, `TicketService` не змінюється.
- `ITicketRepository` / `InMemoryTicketRepository` сховище в пам'яті.
- `TicketService` уся бізнес-логіка, контролер лише викликає сервіс.
- Retry: при помилці `Attempts++`, статус `Failed`, текст у `LastError`. Максимум 3 спроби.
- Ідемпотентність: `notify` не створює нових сповіщень, лише повторює Pending/Failed, які не вичерпали ліміт.

## Правила каналів

- Email: помилка, якщо заголовок порожній
- SMS: помилка для тікетів з пріоритетом Low
- Push: помилка, якщо заголовок довший за 50 символів