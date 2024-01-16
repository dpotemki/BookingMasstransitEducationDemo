# Booking MassTransit Educational Example

## **Описание проекта**
Дэмо версия микросервисной архитектуры, outbox (InMemory) и Saga(rabbitmq + posgresql) паттерна при помощи библиотеки MassTransit. 

Данный проект был создан исключительно для тех кто хочет быстро разобраться как это работает. Все процессы в проекте упрощены. И не являются продакшен, о чем в проекте часто говорится в комментариях. (как например тот момент что сага не финализируется)

## Суть проекта
Приложение по бронированию путевок. Парралельно отправляются команды на бронирование различным сервисам, они отвечают через N времени. 
Если кто то ответил отказом, то остальным отпраляется компенсационная транзакция об отмене и сага завершается отменой.
Только после того как все сервисы ответили положительно, сага считается завершенной положительно.

#### В общем случае, существует два подхода к отправке сообщений в очереди при реализации распределенных систем:

1) Отправка всех сообщений сразу, а затем ожидание ответов в состоянии ожидания подтверждения.
2) Отправка сообщений по одному и ожидание ответа от каждого сервиса перед отправкой следующего сообщения.
Первый подход

При первом подходе, сага отправляет все сообщения в очереди сразу. Затем сага переходит в состояние ожидания подтверждения, где она ожидает ответов от всех сервисов. Если от одного из сервисов не получен ответ в течение заданного времени, сага переходит в состояние отказа.

Пример:

Cага должна забронировать билет на самолет, номер в отеле и трансфер. В этом случае, сага отправит три сообщения в очереди одновременно. Затем она перейдет в состояние ожидания подтверждения, где она будет ждать ответов от трех сервисов. Если от одного из сервисов не получен ответ в течение 30 минут, сага перейдет в состояние отказа.

Плюсы первого подхода:

Простота реализации.
Скорость обработки.
Минусы первого подхода:

Риск отказа всей операции, если один из сервисов не ответит в течение заданного времени.
Сложность обработки отказов.
Второй подход

При втором подходе, сага отправляет сообщения в очереди по одному. После отправки каждого сообщения, сага ожидает ответа от сервиса. Если от сервиса получен ответ, сага переходит к отправке следующего сообщения. Если от сервиса не получен ответ в течение заданного времени, сага переходит в состояние отказа.

Пример:

В том же примере, что и выше, сага будет отправлять сообщения в следующем порядке:

Сообщение для сервиса бронирования билетов на самолет.
Сообщение для сервиса бронирования номера в отеле.
Сообщение для сервиса бронирования трансфера.
Плюсы второго подхода:

Снижение риска отказа всей операции.
Упрощение обработки отказов.
Минусы второго подхода:

Снижение скорости обработки.
Сложность реализации.
Выбор подхода

Выбор подхода к отправке сообщений в очереди зависит от конкретных требований к системе. Если система предъявляет высокие требования к скорости обработки, то предпочтительнее использовать первый подход. Если же система предъявляет высокие требования к надежности, то предпочтительнее использовать второй подход.

В нашем случае, мы используем первыйй подход. Этот подход позволяет базово понять как это работает.После того как вы разберетесь как это работает, вот ссылка https://github.com/qoollo/masstransit-demo/tree/main отличной реализации второго подхода который так же включает в себя мониторинг и логирование , без чего в реальной микросервисной архитектуре никуда!!!
