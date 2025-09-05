using Android.App;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using AndroidX.Core.Content;
using System.IO;
using System.Reflection;

namespace SleepTimer.Models
{
    public static class DrawableExtractor
    {
        public static void SaveSystemDrawables()
        {
            var context = Android.App.Application.Context;
            var drawableType = typeof(Android.Resource.Drawable);

            // Loop through all public static fields in Android.Resource.Drawable
            var fields = drawableType.GetFields(BindingFlags.Public | BindingFlags.Static);

            foreach (var field in fields)
            {
                try
                {
                    int resId = (int)field.GetValue(null);
                    Drawable drawable = context.GetDrawable(resId);

                    if (drawable == null)
                        continue;

                    Bitmap bitmap;

                    if (drawable is BitmapDrawable bitmapDrawable)
                    {
                        bitmap = bitmapDrawable.Bitmap;
                    }
                    else
                    {
                        // Handle VectorDrawable and other types
                        bitmap = Bitmap.CreateBitmap(
                            drawable.IntrinsicWidth > 0 ? drawable.IntrinsicWidth : 64,
                            drawable.IntrinsicHeight > 0 ? drawable.IntrinsicHeight : 64,
                            Bitmap.Config.Argb8888);

                        Canvas canvas = new Canvas(bitmap);
                        drawable.SetBounds(0, 0, canvas.Width, canvas.Height);
                        drawable.Draw(canvas);
                    }

                    // Save to app's cache directory
                    //var filePath = System.IO.Path.Combine(context.CacheDir.AbsolutePath, $"{field.Name}.png");

                    //var path = System.IO.Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, "MyIcons");
                    

                    var path = System.IO.Path.Combine(
                        Android.OS.Environment.GetExternalStoragePublicDirectory(
                            Android.OS.Environment.DirectoryDownloads
                        ).AbsolutePath,
                        "MyIcons"
                    );

                    Directory.CreateDirectory(path);
                    var filePath = System.IO.Path.Combine(path, $"{field.Name}.png");

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        bitmap.Compress(Bitmap.CompressFormat.Png, 100, stream);
                    }

                    System.Diagnostics.Debug.WriteLine($"Saved {field.Name} to {filePath}");
                }
                catch
                {
                    // Some drawables may fail; skip them
                    continue;
                }
            }
        }
    }
}
