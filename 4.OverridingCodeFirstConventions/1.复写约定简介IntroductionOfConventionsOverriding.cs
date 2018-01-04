//Q: 什么是约定？
//A: 约定(Conventions)在CodeFirstWorkflow中是指一种从Model到数据库的自动映射方式，即Model中的对象属性与数据库中的对象属性的转换规则。EF提供了一套
  //默认的约定(Conventions)，如Model中的String类型对应数据库中的nvarchar(MAX)，String类型的对象转到数据库中默认为可空(Nullable: True)等等。

//Q: 为什么要有约定？
//A: 约定提供了一套转换准则，是EF运作的基础之一，EF内置了一套默认的规则。

//Q: 为什么要复写约定？
//A: 很多情况下我们对Model和数据库之间的映射有自定义要求。如默认约定Model到数据库生成的表名为其类名的复数，如基于Course类的表默认的约定将其命名为
  //Courses，如果我们想让这表的名字自动命名为tbl_Courses等等就需要将更改默认约定来达到我们的需求。

//Q: 如何复写约定？
//A: 在DBW中有两种复写方法，即数据注释(DataAnnotations)和FluentAPI。接下来几节将详细讲解这两种复写方法。

//暂时想到这么多，最后更新2018/01/03
