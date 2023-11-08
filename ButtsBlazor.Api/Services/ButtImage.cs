﻿using ButtsBlazor.Api.Model;

namespace ButtsBlazor.Server.Services;



public record ButtImage(WebPath Path, DateTime Created, int Index, bool IsLatest=false)
{
    public static readonly ButtImage Empty = new(new WebPath("android-chrome-512x512.png"), DateTime.MinValue, 0, false);

    public static ButtImage From(ImageEntity entity) => new ButtImage(entity.Path, entity.CreationDate, entity.RowId);

}