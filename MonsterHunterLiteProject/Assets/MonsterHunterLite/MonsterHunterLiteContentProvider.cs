using RoR2.ContentManagement;
using UnityEngine;
using RoR2;
using System.Collections;
namespace MonsterHunterLite
{
    public class MonsterHunterLiteContent : IContentPackProvider
    {
        public string identifier => MonsterHunterLiteMain.GUID;

        public static ReadOnlyContentPack readOnlyContentPack => new ReadOnlyContentPack(MonsterHunterLiteContentPack);
        internal static ContentPack MonsterHunterLiteContentPack { get; } = new ContentPack();

        public IEnumerator LoadStaticContentAsync(LoadStaticContentAsyncArgs args)
        {
            var asyncOperation = AssetBundle.LoadFromFileAsync(MonsterHunterLiteMain.assetBundleDir);
            while(!asyncOperation.isDone)
            {
                args.ReportProgress(asyncOperation.progress);
                yield return null;
            }

            //Write code here to initialize your mod post assetbundle load
        }
        public IEnumerator GenerateContentPackAsync(GetContentPackAsyncArgs args)
        {
             ContentPack.Copy(MonsterHunterLiteContentPack, args.output);
            args.ReportProgress(1f);
            yield break;
        }
        public IEnumerator FinalizeAsync(FinalizeAsyncArgs args)
        {
            args.ReportProgress(1f);
            yield break;
        }
        private void AddSelf(ContentManager.AddContentPackProviderDelegate addContentPackProvider)
        {
            addContentPackProvider(this);
        }
        internal MonsterHunterLiteContent()
        {
            ContentManager.collectContentPackProviders += AddSelf;
        }
    }
}
