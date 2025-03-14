动画修改贴图：

把序列帧动画导入到Assets/Resources/主角，记得修改名称，最好导出成一整张精灵图。

分割一整张精灵图：

![image-20250314163226235](C:\Users\dtj\AppData\Roaming\Typora\typora-user-images\image-20250314163226235.png)

然后点击sprite editor 切片-自动-应用

如何应用到玩家？

1：打开Assets/Scenes/动画测试 

在层级中找到“武器攻击动画”  双击它

![image-20250314163707691](C:\Users\dtj\AppData\Roaming\Typora\typora-user-images\image-20250314163707691.png)

将动画帧拖上去 点击播放就可以看到效果 帧上面第一个事件是攻击判定 放在判定帧 第二个是攻击结束 要放在动画最后