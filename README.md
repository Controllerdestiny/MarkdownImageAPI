# MarkdownImageAPI

一个将 Markdown 文本转为图片的程序

## 注意事项

- 此程序基于 Chrome 内核渲染图片
- 此程序依托于[oiapi.net](http://docs.oiapi.net)
- 如果你有能力也可自己建站，其中样 css 式文件可在 [github-markdown-css](https://github.com/sindresorhus/github-markdown-css) 上找到

## 启动参数

- `port` 设置端口
- `-chrome` Chrome 路径

## 配置

```json
{
  "端口": 7776,
  "日志路径": "Log",
  "日志大小": 32,
  "启用无头": false,
  "内核启动参数": []
}
```
