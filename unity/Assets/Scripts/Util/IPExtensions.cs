﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class IPRectExtensions
{
    public static Rect CloneAndScale(this Rect rect, float scale)
    {
        float left = rect.xMin;
        float bottom = rect.yMin;
        float width = rect.width;
        float height = rect.height;

        //we now have the original rect, check if we need to scale it
        if (scale != 1.0f)
        {
            //scale the width and height
            float newWidthMag = rect.width * scale;
            float newHeightMag = rect.height * scale;

            //find the difference
            float widthDelta = rect.width - newWidthMag;
            float heightDelta = rect.height - newHeightMag;

            width = newWidthMag;
            height = newHeightMag;

            //move the left and top to accomodate the new size (grow/shrink around center)
            left += widthDelta / 2;
            bottom += heightDelta / 2;
        }

        //return a rect with the calculated top, left, and sizes
        return new Rect(left, bottom, width, height);
    }
}
