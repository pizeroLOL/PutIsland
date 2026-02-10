# PutIsland

允许其他软件使用 http post `http://127.0.0.1:<端口>/<id>` 更新 ClassIsland 组件。

## 使用方式

添加好插件和组件

```shell
curl -X POST http://localhost:36000/<id> -d <文字内容>
```

## TODO

- [ ] 动态 host / post
- [x] 支持多 id
- [x] 多组件同时侦听