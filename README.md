# Angie-C\#

C# 实现的 nodejs-webframework-like Web 框架，尚未实现 AngieRequest、AngieResponse。

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

这是因为，use 的每一次调用将像这样包装一个入口:

接下来，每次请求都将进入这个包装，并从中间件栈顶递归包装到栈底，并生成一个新的中间件入口:

```cs
this.entry = (req, res) =>
{
  Stack<AngieMiddleware> middlewaresCloned = new Stack<AngieMiddleware>(new Stack<AngieMiddleware>(this.middlewares));

  AngieMiddleware eachMiddleware = null;
  AngieNextMiddleware previousNext = null;
  while (middlewaresCloned.TryPop(out eachMiddleware))
  {
    var currentMiddleware = eachMiddleware;
    var next = previousNext;
    previousNext = () =>
    {
      currentMiddleware(req, res, next);
    };
  }
  previousNext();
};
```

他几乎等同于 Mirai-js 的这一段中间件实现，只不过后者是同步协程:

[Mirai-js/src/Middleware.js#L596](https://github.com/Drincann/Mirai-js/blob/master/src/Middleware.js#L596)

```js
let entry = (req, res) => {
  return this.middlewares.reduceRight((next, currentMiddleware) => {
    return async () => await currentMiddleware(req, res, next);
  }, undefined)();
};
```
