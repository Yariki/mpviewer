using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Primitives;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using MPViewer.Interfaces;
using MPViewer.Models;

namespace MPViewer.Helpers
{
    [Export(typeof(IMPLoadManager))]
    public class MPLoadManager : IMPLoadManager
    {

        private Thread loadingThumbhailsThread = null;
        private Thread loadingFullSizeThread = null;

        private AutoResetEvent loadingThumbnailEvent = new AutoResetEvent(false);
        private AutoResetEvent loadingFullSizeEvent = new AutoResetEvent(false);


        private Queue<MPPhoto> queueThumbhails = new Queue<MPPhoto>();
        private Queue<MPPhoto> queueFullSize = new Queue<MPPhoto>();
        
        public MPLoadManager()
        {
            loadingThumbhailsThread = new Thread(LoadingThumbnailsThread);
            loadingThumbhailsThread.IsBackground = true;
            loadingThumbhailsThread.Priority = ThreadPriority.AboveNormal;
            loadingThumbhailsThread.Start();


            loadingFullSizeThread = new Thread(LoadingFullsizeThread);
            loadingFullSizeThread.IsBackground = true;
            loadingFullSizeThread.Priority = ThreadPriority.BelowNormal;
            loadingFullSizeThread.Start();
        }


        public void LoadThumbnail(MPPhoto photo)
        {
            lock (queueThumbhails)
            {
                queueThumbhails.Enqueue(photo);
            }
            loadingThumbnailEvent.Set();
        }

        public void LoadFullSize(MPPhoto photo)
        {
            lock (queueFullSize)
            {
                queueFullSize.Enqueue(photo);
            }
            loadingFullSizeEvent.Set();
        }

        private BitmapSource GetBitmapSource(string filename, bool isThumbnail = true)
        {
            BitmapSource bitmapSource = null;
            if (string.IsNullOrEmpty(filename))
            {
                return bitmapSource;
            }
            Stream bitmapStream = null;
            try
            {
                bitmapStream = File.OpenRead(filename);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
            }

            if (bitmapStream != null)
            {
                try
                {
                    if (isThumbnail)
                    {
                        BitmapFrame bitmapFrame = BitmapFrame.Create(bitmapStream);
                        bitmapSource = bitmapFrame.Thumbnail;

                        if (bitmapSource == null)
                        {
                            TransformedBitmap thumbnail = new TransformedBitmap();
                            thumbnail.BeginInit();
                            thumbnail.Source = bitmapFrame as BitmapSource;

                            int pixelH = bitmapFrame.PixelHeight;
                            int pixelW = bitmapFrame.PixelWidth;
                            int decodeH = 240;
                            int decodeW = (bitmapFrame.PixelWidth * decodeH) / pixelH;
                            double scaleX = decodeW / (double) pixelW;
                            double scaleY = decodeH / (double) pixelH;
                            TransformGroup transformGroup = new TransformGroup();
                            transformGroup.Children.Add(new ScaleTransform(scaleX, scaleY));
                            thumbnail.Transform = transformGroup;
                            thumbnail.EndInit();

                            WriteableBitmap writable = new WriteableBitmap(thumbnail);
                            writable.Freeze();
                            bitmapSource = writable;
                        }
                    }
                    else
                    {
                        BitmapImage bitmapImage = new BitmapImage();
                        bitmapImage.BeginInit();
                        bitmapImage.StreamSource = bitmapStream;
                        bitmapImage.EndInit();
                        bitmapImage.Freeze();
                        bitmapSource = bitmapImage;
                    }
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.ToString());
                }
            }
            return bitmapSource;
        }

        private void SetThumbhailImage(MPPhoto photo, BitmapSource bitmapSource)
        {
            Application.Current.Dispatcher.BeginInvoke((Action) (() =>
            {
                photo.Thumbnail = bitmapSource;
            }));
            
        }

        private void SetFullSizeImage(MPPhoto photo, BitmapSource bitmapSource)
        {
            Application.Current.Dispatcher.BeginInvoke((Action) (() =>
            {
                photo.Image = bitmapSource;
            }));
        }


        private void LoadingThumbnailsThread()
        {
            do
            {
                loadingThumbnailEvent.WaitOne();
                MPPhoto photo = null;

                do
                {
                    lock (queueThumbhails)
                    {
                        photo = queueThumbhails.Count > 0 ? queueThumbhails.Dequeue() : null;
                    }

                    if (photo != null)
                    {
                        var bitmapSource = GetBitmapSource(photo.PhotoPath);
                        SetThumbhailImage(photo,bitmapSource);
                    }
                    

                } while (photo != null);



            } while (true);
        }

        private void LoadingFullsizeThread()
        {
            do
            {
                loadingFullSizeEvent.WaitOne();
                MPPhoto photo = null;

                do
                {
                    lock (queueFullSize)
                    {
                        photo = queueFullSize.Count > 0 ? queueFullSize.Dequeue() : null;
                    }
                    if (photo != null)
                    {
                        var bitmapSource = GetBitmapSource(photo.PhotoPath, false);
                        SetFullSizeImage(photo,bitmapSource);
                    }


                } while (photo != null);



            } while (true);
        }
            

        
    }
}