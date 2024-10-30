# vk_user_info
OTRPO lab3
Приложение написано на языке C# с использованием `.NET 8.0`



## Установка пакетов для сборки и выполнения исходного кода

### Для Debian

В первой команде подставьте версию Debian (12, 11, 10)
```sh
wget https://packages.microsoft.com/config/debian/ВЕРСИЯ-DEBIAN/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb
sudo apt-get update && \
  sudo apt-get install -y dotnet-sdk-8.0
sudo apt-get install -y dotnet-runtime-8.0
```

### Для Ubuntu
```sh
sudo apt-get update && \
  sudo apt-get install -y dotnet-sdk-8.0
sudo apt-get install -y dotnet-runtime-8.0
```

### Для Windows
Скачать и установить .NET SDK 8.0 (`https://dotnet.microsoft.com/en-us/download/dotnet/8.0`)

## Запуск приложения
Запуск осуществляется с помощью команды `dotnet run` в директории репозитория
Без параметров (будут подставлены дефолтные: id astraz1one, и путь )
```sh
dotnet run --access-token ВАШ-ТОКЕН
```

С vk id пользователя
```sh
dotnet run --access-token ВАШ-ТОКЕН --user-id ID-ПОЛЬЗОВАТЕЛЯ
```

С vk user id и путём для сохранения результата
```sh
dotnet run --access-token ВАШ-ТОКЕН --user-id ID-ПОЛЬЗОВАТЕЛЯ --path C:/some_path/result.json
```

Файл с результатами будет сохранён по пути vk_user_data.json относительно в директории репозитория (если путь не был указан вручную)

## Q&A

Почему был использован именно .NET 8.0, а не .NET 6.0 или .NET Framework 4.7.2?
Хоть .NET Framework 4.7.2 изначально установен на Windows 10, 11 как системный пакет, но данный фреймворк не является кроссплатформенным (Windows only). 
Версия .NET 6.0 является довольно популярной, но не была выбрана из-за сложностей установки в ubuntu 
(для версии 24.04 .NET 8.0 доступен во встроенном репозитории пакетов, а .NET 6.0 доступен только в репозитории пакетов backports, из-за чего приходится делать лишние действия)

## Дополнительная информация

### Развёртывание приложения

```
dotnet publish -c Release -r <RID> --self-contained true
```

#### Для Windows

RID: win-x64 

#### Для Linux

RID: linux-x64 

### Необходимые пакеты для запуска приложения

https://github.com/dotnet/core/blob/main/release-notes/8.0/linux-packages.md