# PutIsland

允许其他软件使用 http post `http://<监听网址>:<端口>/<id>` 更新 ClassIsland 组件，非
localhost 需要已管理员权限运行。

## 使用方式

添加好插件和组件

```shell
curl -X POST http://<监听网址>:<端口>/<id> -d <文字内容>
```

## TODO

- [x] 动态 host / post
- [x] 支持多 id
- [x] 多组件同时侦听