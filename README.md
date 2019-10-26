# QuestHelper - рабочее название проекта GoSh!
Цели проекта:
- Дать возможность пользователю отметить интересные места, с фото и текстовым описанием
- Опубликовать собранные данные друзьям для просмотра и совместного редактирования

Небольшое описание в статье:
https://habr.com/ru/post/458272/

Приложение в Google Play:
https://play.google.com/store/apps/details?id=com.sd.gosh&hl=ru

Endpoint backend:
http://igosh.pro/api/swagger/index.html

Солюшен содержит решение как серверной части, так и мобильное приложение.
Структура солюшена:
- Проект QuestHelper. Основной проект для мобильного приложения с кросс-платформенной  бизнес-логикой на Xamarin Forms.
- Проект QuestHelper.Android. Проект для сборки нативного приложения Android.
- Проект QuestHelper.iOs. Проект для сборки нативного приложения iOs.
- Проект QuestHelper.LocalDB. Проект моделей для БД в мобильном приложении на базе Realm.
- Проект QuestHelper.Server. Основной проект для сборки сервера на базе WebApi .Net Core 2.1
- Проект QuestHelper.SharedModelsWS. Проект с моделями контрактов, общими для API и клиента.
- Проект QuestHelper.Tests. Юнит-тесты для мобильного приложения.
- Проект QuestHelperServer.Tests. Юнит-тесты для серверной части.
