using System;

class TestClass
{
  public static void Main()
  {
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
  }
}
