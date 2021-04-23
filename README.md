# tinyMangaViewer
a tiny manga viewer 

重温WPF制作的小工具之一极其简单的漫画查看器。

可直接查看zip,7z,rar以及目录里的漫画，功能极简，图片自适应显示，没有添加任何图片处理功能，甚至没有添加放大缩小及平移功能。图片读取也没添加缓存预读功能，一切以简单快速完整为主。

# 安装使用

程序本体使用VS2019编译后即可运行。

shell扩展部分（也就是关联文件资源管理器中相关文件类型的右键菜单）编译好后需要把所有dll复制到本体exe所在目录，并且包括复制install.bat、uninstall.bat，然后用管理员身份运行install.bat安装扩展即可，install.bat使用的是64位regasm进行注册，如果想改成32位则需要修改install.bat，引用32位regasm。


# 采用的第三方库

ViewModel自动属性工具 PropertyChanged.Fody https://github.com/Fody/PropertyChanged

Shell扩展部分 SharpShell https://github.com/dwmkerr/sharpshell

rar及7z解压部分 SharpCompress https://github.com/adamhathcock/sharpcompress
