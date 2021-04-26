﻿using System;

namespace Arches
{
    public static class Semantics
    {
        public static Image FilterColor(Image image, int color)
        {
            Image output = new Image(image.x, image.y, image.w, image.h);

            for (int i = 0; i < output.data.Length; i++)
            {
                output.data[i] = (image.data[i] == color) ? color : 0;
            }

            return output;
        }

        public static Image Recolor(Image image, int color)
        {
            Image output = new Image(image.x, image.y, image.w, image.h);

            for (int i = 0; i < output.data.Length; i++)
            {
                output.data[i] = (image.data[i] != 0) ? color : 0;
            }

            return output;
        }

        public static Image Orthogonal(Image inputImage, int option)
        {
            int[,] inputArr = inputImage.toArray();
            int[,] outputArr = null;

            int max_index_d0 = -1;
            int max_index_d1 = -1;
            switch (option)
            {
                case 0: // Y_AXIS
                    max_index_d0 = inputArr.GetUpperBound(0);
                    max_index_d1 = inputArr.GetUpperBound(1);
                    // Step 1: Flip the rows along the Y_AXIS (horizontal)
                    outputArr = new int[max_index_d0 + 1, max_index_d1 + 1];
                    for (int i = 0; i <= max_index_d0; i++)
                    {
                        for (int j = 0; j <= max_index_d1; j++)
                        {
                            outputArr[i,j] = inputArr[i, max_index_d1 - j];
                        }
                    }
                    break;
                case 1: // X_AXIS
                    max_index_d0 = inputArr.GetUpperBound(0);
                    max_index_d1 = inputArr.GetUpperBound(1);
                    // Step 1: Flip the rows along the X_AXIS (vertical)
                    outputArr = new int[max_index_d0 + 1, max_index_d1 + 1];
                    for (int i = 0; i <= max_index_d0; i++)
                    {
                        for (int j = 0; j <= max_index_d1; j++)
                        {
                            outputArr[i,j] = inputArr[max_index_d0 - i, j];
                        }
                    }
                    break;
                case 2: // ROT_90
                    // Step 0: Avoid re-computing upper bounds throughout (note dimensions are flipped)
                    max_index_d0 = inputArr.GetUpperBound(1);
                    max_index_d1 = inputArr.GetUpperBound(0);
                    // Step 1: Find the transpose of the matrix
                    int[,] tmpArr = new int[max_index_d0 + 1, max_index_d1 + 1];
                    for (int i = 0; i <= max_index_d0; i++)
                    {
                        for (int j = 0; j <= max_index_d1; j++)
                        {
                            tmpArr[i,j] = inputArr[j,i];
                        }
                    }
                    // Step 2: Reverse rows in the transpose
                    outputArr = new int[max_index_d0 + 1, max_index_d1 + 1];
                    for (int i = 0; i <= max_index_d0; i++) {
                        for (int j = 0; j <= max_index_d1; j++) {
                            outputArr[i,j] = tmpArr[i,max_index_d1 - j];
                        }
                    }
                    // Step 3: Profit (Convert this back into an Image)
                    break;
                default:
                    break;
            }
            // return null if option wasn't supported, or our image
            if (outputArr == null) {
                return null;
            }
            return new Image(outputArr);
        }

        public static Image Origin(Image image)
        {
            Image output = image.Clone() as Image;
            output.x = 0;
            output.y = 0;
            return output;
        }

        public static Image Identity(Image image)
        {
            return image;
        }

        public static Image Compose(Image a, Image b)
        {
            int left = Math.Min(a.x, b.x);
            int right = Math.Max(a.x + a.w, b.x + b.w);
            int top = Math.Min(a.y, b.y);
            int bottom = Math.Max(a.y + a.h, b.y + b.h);
            Image ret = new Image(left, top, right - left, bottom - top);
            for (int ay = a.y; ay < a.y + a.h; ay++)
            {
                for (int ax = a.x; ax < a.x + a.w; ax++)
                {
                    ret.setPixel(ax, ay, a.getPixel(ax, ay));
                }
            }
            for (int ay = b.y; ay < b.y + b.h; ay++)
            {
                for (int ax = b.x; ax < b.x + b.w; ax++)
                {
                    ret.setPixel(ax, ay, b.getPixel(ax, ay));
                }
            }
            return ret;
        }

        public static Image Compress(Image image)
        {
            int minrow = image.h;
            for(int i = 0; i < image.h; i++)
            {
                for (int j = 0; j < image.w; j++)
                {
                    if (image.data[i * image.w + j] != 0)
                    {
                        minrow = i;
                        break;
                    }
                }
                if (minrow != image.h)
                {
                    break;
                }
            }
            int mincol = image.w;
            for (int j = 0; j < image.w; j++)
            {
                for (int i = 0; i < image.h; i++)
                {
                    if (image.data[i * image.w + j] != 0)
                    {
                        mincol = j;
                        break;
                    }
                }
                if (mincol != image.w)
                {
                    break;
                }
            }
            int maxrow = -1;
            for (int i = image.h - 1; i >= 0; i--)
            {
                for (int j = 0; j < image.w; j++)
                {
                    if (image.data[i * image.w + j] != 0)
                    {
                        maxrow = i;
                        break;
                    }
                }
                if (maxrow != -1)
                {
                    break;
                }
            }
            int maxcol = -1;
            for (int j = image.w - 1; j >= 0; j--)
            {
                for (int i = 0; i < image.h; i++)
                {
                    if (image.data[i * image.w + j] != 0)
                    {
                        mincol = j;
                        break;
                    }
                }
                if (maxcol != -1)
                {
                    break;
                }
            }
            Image ret = new Image(image.x + mincol, image.y + minrow, maxcol - mincol + 1, maxrow - minrow + 1);
            // TODO: COPY DATA
            return ret;
        }
    }
}