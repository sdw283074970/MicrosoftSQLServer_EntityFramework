/*本篇记录了一些PM的常用命令参数，持续更新*/

/*1.安装EntityFramework及对应版本*/
/* install-package EntityFramework -Version:6.x.x*/

/*2.启用Migration(一个项目终身只启用一次)*/
/* enable-migrations */

/*3.建立迁移文件*/
/* add-migration [MigrationName] */

/*4.将最新迁移文件更新至数据库*/
/* update-database */

/*5.将数据库降级至目标时间点(目标迁移文件)*/
/* update-database -TargetMigration:[TargetMigrationName] */

/*6.将所有迁移文件转化为Sql文件*/
/* Update-Database -Script -SourceMigration:0 */

/*7.将目标范围迁移文件转化为Sql文件*/
/* Update-Database -Script -SourceMigration:[Migr1] -TargetMigration:[Migr2] */
