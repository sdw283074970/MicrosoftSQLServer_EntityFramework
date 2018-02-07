//Q: 什么是LINQPad？
//A: LINQPad是一个大神开发的应用程序。这个程序能加强在EntityFramework与数据库之间工作的效率。进入官网www.linqpad.net可下载这一程序。
.
//Q: 如何使用LINQPad？
//A: 首先要先导入connection，点击窗口左边的AddConnection，在弹出的ChooseDataContext对话框中按情况选择，这里我们选择使用已有程序集的数据，在接下来
  //的EntityFrameworkDbContextConnection设置程序集路径(*\bin\Debug\*.exe)、名称，再设置App.config文件路径(为了获得connectionString)。最后点击
  //“测试”进行测试。然后就能在左侧窗口中看到数据库了。
  
  //中上窗口为查询窗口，中下窗口为结果窗口，可以查看普通结果、SQL语句和IL(中间语言)。查询窗口中可以贴上LINQ语句，然后点击运行就能在直接查看各种结果。
    //LINQPAD的最大好处就是能够及时查看碎片化的语句的运行结果，非常方便。
  
  //LINQPAD左下角也有更详细的简易教程。

//暂时想到这么多，最后更新2018/02/07
