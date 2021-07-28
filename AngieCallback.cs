namespace Web
{
  public delegate void AngieMiddleware(AngieRequest request, AngieResponse response, AngieNextMiddleware next);

  public delegate void AngieEntry(AngieRequest request, AngieResponse response);

  public delegate void AngieNextMiddleware();

  public delegate System.Threading.ThreadStart AngieThread(System.Net.Sockets.Socket clientSocket);
}