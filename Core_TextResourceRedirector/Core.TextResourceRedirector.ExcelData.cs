﻿using XUnity.AutoTranslator.Plugin.Core;
using XUnity.AutoTranslator.Plugin.Core.AssetRedirection;
using XUnity.AutoTranslator.Plugin.Core.Utilities;
using XUnity.ResourceRedirector;

namespace KK_Plugins
{
    public class ExcelDataResourceRedirector : AssetLoadedHandlerBase<ExcelData>
    {
        protected override string CalculateModificationFilePath(ExcelData asset, IAssetOrResourceLoadedContext context) =>
            context.GetPreferredFilePathWithCustomFileName(@"BepInEx\translation", asset, "translation.txt").Replace(@"abdata\", "").Replace(".unity3d", "");

        protected override bool DumpAsset(string calculatedModificationPath, ExcelData asset, IAssetOrResourceLoadedContext context)
        {
            var cache = new SimpleTextTranslationCache(calculatedModificationPath, false);

            foreach (var param in asset.list)
            {
                for (int i = 0; i < param.list.Count; i++)
                {
                    var key = param.list[i];
                    if (!key.IsNullOrWhiteSpace() && LanguageHelper.IsTranslatable(key))
                    {
                        cache.AddTranslationToCache(key, key);
                    }
                }
            }

            return true;
        }

        protected override bool ReplaceOrUpdateAsset(string calculatedModificationPath, ref ExcelData asset, IAssetOrResourceLoadedContext context)
        {
            var cache = new SimpleTextTranslationCache(calculatedModificationPath, true);

            foreach (var param in asset.list)
            {
                for (int i = 0; i < param.list.Count; i++)
                {
                    var key = param.list[i];
                    if (!key.IsNullOrWhiteSpace())
                    {
                        if (cache.TryGetTranslation(key, true, out var translated))
                        {
                            param.list[i] = translated;
                        }
                        else if (IsDumpingEnabled && LanguageHelper.IsTranslatable(key))
                        {
                            cache.AddTranslationToCache(key, key);
                        }
                    }
                }
            }

            return true;
        }

        protected override bool ShouldHandleAsset(ExcelData asset, IAssetOrResourceLoadedContext context) => !context.HasReferenceBeenRedirectedBefore(asset);
    }
}