# Angie-C\#

C# 实现的 nodejs-webframework-like Web 框架

与 [Angie-java]([/](https://github.com/Drincann/Angie-java)) 的 HashMap 路由不同，Angie-C\# 仅带有一个真实的同步中间件实现。

就像 [demo.cs](./demo.cs) 那样，next 调用将在下游中间件执行结束后返回。

```cs
// print 1 2 3
var app = new Web.Angie();
app.use((req, res, next) =>
{
  Console.WriteLine(1);
  next();
  Console.WriteLine(3);
});

app.use((req, res, next) =>
{
  Console.WriteLine(2);
});

app.listen(80);
```
