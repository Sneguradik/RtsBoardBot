# RtsBoardBot

**RtsBoardBot** — это .NET-проект, предназначенный для работы с торговыми данными RTS с использованием Tinkoff API.

## 🔧 Требования

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/)
- Аккаунт в Tinkoff и токен доступа к API

## ⚙️ Переменные среды

Перед запуском необходимо задать переменную среды:

| Название       | Описание                        |
|----------------|----------------------------------|
| `TinkoffToken` | API-токен для доступа к Tinkoff |


## Запуск проекта

### 🔹 **Локальный запуск**

1. Установите необходимые зависимости:
```sh
dotnet restore
```
2. Запустите приложение:
```sh
dotnet run
```

### 🔹 **Запуск в Docker**

1. Соберите Docker-образ (Сборка производится из корня проекта):
- Windows:
```bash
docker build -f Worker\Dockerfile -t rtsbot .
```
- Unix:
```sh
docker build -f Worker/Dockerfile -t rtsbot .
```

2. Запустите контейнер:
```sh
docker run --env TinkoffToken=<API_KEY> --name rtsbot rtsbot 
```


## Развёртывание
Для деплоя можно использовать `Docker Compose`, `Kubernetes`, `systemd` или любую другую систему управления сервисами.

## Контакты
Если у вас возникли вопросы, обратитесь к разработчику или создайте issue в репозитории.