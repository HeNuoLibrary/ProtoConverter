proto文件转换为cs脚本插件，使用时运行run.bat，将要转化的proto文件放入proto文件夹下


Proto文件编写说明

3.option optimize_for = SPEED;
optimize_for是文件级别的选项，Protocol Buffer定义三种优化级别SPEED/CODE_SIZE/LITE_RUNTIME。缺省情况下是SPEED。
SPEED: 表示生成的代码运行效率高，但是由此生成的代码编译后会占用更多的空间。
CODE_SIZE: 和SPEED恰恰相反，代码运行效率较低，但是由此生成的代码编译后会占用更少的空间，通常用于资源有限的平台，如Mobile。
LITE_RUNTIME: 生成的代码执行效率高，同时生成代码编译后的所占用的空间也是非常少。这是以牺牲Protocol Buffer提供的反射功能为代价的。

 

4:option java_multiple_files=true; 让每个消息都独立生成文件，减少单个文件的大小