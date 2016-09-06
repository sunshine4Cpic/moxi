using moxiCommunity.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Hosting;

namespace moxiCommunity.Controllers {
    public class CacheExtend {


        private static int s_NodesCacheDependencyFlag = 0;
        private static int s_BannersCacheDependencyFlag = 0;

        private const string nodeFilePath = "~/App_Data/node.json";
        private const string bannerFilePath = "~/App_Data/banner.json";

        private static List<proNode> _nodes;
        private static List<banner> _banners;

        public static List<proNode> nodes {
            // s_RunOptions 的初始化放在Init方法中了，会在Global.asax的Application_Start事件中调用。
            get { return _nodes; }
        }

        public static List<banner> banners {
            // s_RunOptions 的初始化放在Init方法中了，会在Global.asax的Application_Start事件中调用。
            get { return _banners; }
        }
        public static void LoadNodes() {

            var _jsonFilePath = HostingEnvironment.MapPath(nodeFilePath);

            List<proNode> nodes;

            try {
                var json = System.IO.File.ReadAllText(_jsonFilePath);
                nodes = JsonConvert.DeserializeObject<List<proNode>>(json);
            } catch (Exception) {
                nodes = _nodes;//报错取原值
            }

            _nodes = nodes;

            int flag = System.Threading.Interlocked.CompareExchange(ref s_NodesCacheDependencyFlag, 1, 0);

            // 确保只调用一次就可以了。
            if (flag == 0) {
                // 让Cache帮我们盯住这个配置文件。
                CacheDependency dep = new CacheDependency(_jsonFilePath);
                HttpRuntime.Cache.Insert("nodes", "1", dep,
                    Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, nodesUpdateCallback);
            }

        }


        public static void nodesUpdateCallback(string key, CacheItemUpdateReason reason, out object expensiveObject, out CacheDependency dependency, out DateTime absoluteExpiration, out TimeSpan slidingExpiration) {

            // 注意：在这个方法中，不要出现【未处理异常】，否则缓存对象将被移除。

            // 说明：这里我并不关心参数reason，因为我根本就没有使用过期时间
            //        所以，只有一种原因：依赖的文件发生了改变。
            //        参数key我也不关心，因为这个方法是【专用】的。

            expensiveObject = "2";//跟新值无所喂
            dependency = new CacheDependency(HostingEnvironment.MapPath(nodeFilePath));
            absoluteExpiration = Cache.NoAbsoluteExpiration;
            slidingExpiration = Cache.NoSlidingExpiration;

            // 重新加载配置参数
            LoadNodes();
        }

        public static void LoadBanners() {

            var _jsonFilePath = HostingEnvironment.MapPath(bannerFilePath);

            List<banner> banners;

            try {
                var json = System.IO.File.ReadAllText(_jsonFilePath);
                banners = JsonConvert.DeserializeObject<List<banner>>(json);
            } catch (Exception) {
                banners = _banners;//报错取原值
            }

            _banners = banners;

            int flag = System.Threading.Interlocked.CompareExchange(ref s_BannersCacheDependencyFlag, 1, 0);

            // 确保只调用一次就可以了。
            if (flag == 0) {
                // 让Cache帮我们盯住这个配置文件。
                CacheDependency dep = new CacheDependency(_jsonFilePath);
                HttpRuntime.Cache.Insert("banners", "1", dep,
                    Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, bannersUpdateCallback);
            }

        }

        public static void bannersUpdateCallback(string key, CacheItemUpdateReason reason, out object expensiveObject, out CacheDependency dependency, out DateTime absoluteExpiration, out TimeSpan slidingExpiration) {
            // 注意：在这个方法中，不要出现【未处理异常】，否则缓存对象将被移除。

            // 说明：这里我并不关心参数reason，因为我根本就没有使用过期时间
            //        所以，只有一种原因：依赖的文件发生了改变。
            //        参数key我也不关心，因为这个方法是【专用】的。

            expensiveObject = "2";//跟新值无所喂
            dependency = new CacheDependency(HostingEnvironment.MapPath(bannerFilePath));
            absoluteExpiration = Cache.NoAbsoluteExpiration;
            slidingExpiration = Cache.NoSlidingExpiration;

            // 重新加载配置参数
            LoadNodes();
        }
    }



}