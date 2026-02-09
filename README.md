# PutIsland

允许其他软件使用 http post `http://127.0.0.1:<端口>/<id>` 更新 ClassIsland 组件。

目前只支持 http://localhost:36000/123

## 使用方式

添加好插件和组件

```shell
curl -X POST http://localhost:36000/123 -d <文字内容>
```

## TODO

- [ ] 动态 host / post
- [ ] 支持多 id
- [ ] 多组件同时侦听