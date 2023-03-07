redis 集群安装

1.下载最新稳定版redis
wget https://download.redis.io/redis-stable.tar.gz
2.解压
tar -zxvf redis-stable.tar.gz -C /usr/local/redis_cluster/
3.进入解压目录编译安装
cd /usr/local/redis_cluster/redis-stable/
make && make install
报错就先安装gcc 在执行 make distclean && make && make install
4.修改redis.conf配置文件
```
port 6379（每个节点的端口号）
daemonize yes
bind  （绑定当前机器 IP）
dir /usr/local/redis-cluster/端口号/data/
pidfile /var/run/redis_端口号.pid 
cluster-enabled yes（启动集群模式）
cluster-config-file nodes端口号.conf（9001和port要对应）
cluster-node-timeout 15000
appendonly yes
```
5.创建其余配置文件
cd ..
mkdir 6401 6402 6403 6404 6405 #mkdir 640{1,2,3,4,5}
cp /usr/local/redis_cluster/redis-stable/redis.conf 6401 6402 6403 6404 6405
6.依次修改配置文件
vim 全局替换 :%s/6379/6401/g
7.依次启动redis
redis-server /usr/local/redis_cluster/redis-stable/redis.conf;
redis-server /usr/local/redis_cluster/6401/redis.conf;
redis-server /usr/local/redis_cluster/6402/redis.conf;
redis-server /usr/local/redis_cluster/6403/redis.conf;
redis-server /usr/local/redis_cluster/6404/redis.conf;
redis-server /usr/local/redis_cluster/6405/redis.conf
redis-cli -h 127.0.0.1 -p 6401 随便测试一个实例
8.启动redis集群
redis-cli --cluster create 127.0.0.1:6379 127.0.0.1:6401 127.0.0.1:6402 127.0.0.1:6403 127.0.0.1:6404 127.0.0.1:6405 --cluster-replicas 1
9.连接redis集群测试
redis-cli -c -h 127.0.0.1 -p 6401
#检查集群是否正常
redis-cli --cluster check 127.0.0.1:6379
进入到故障节点 清空节点相关数据
cluster reset
flushdb
#添加节点到集群
redis-cli --cluster call 127.0.0.1:6401 cluster forget nodeid
redis-cli --cluster call 127.0.0.1:6401 cluster meet 127.0.0.1 6405
#修复集群
redis-cli --cluster fix 127.0.0.1:6379
#查询集群节点
redis-cli --cluster call 127.0.0.1:6401 cluster nodes

#重新创建集群
redis-cli --cluster reshard 127.0.0.1:6379
ps aux|grep server|grep -v grep | awk '{print $2}'|xargs kill -9
find 6401 6402 6403 6404 6405 -name "dump*" | xargs rm -rf