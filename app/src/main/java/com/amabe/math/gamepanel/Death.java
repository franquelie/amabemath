package com.amabe.math.gamepanel;

import android.content.Context;
import android.graphics.Canvas;
import android.graphics.Paint;

import androidx.core.content.ContextCompat;

import com.example.gameuntitled.R;

/**
 * YouDied is a panel which draws the text You Died to the screen.
 */

public class Death {
    private Context context;

    public Death(Context context) {
        this.context = context;
    }

    public void draw(Canvas canvas) {
        String text = "You Died";

        float x = 800;
        float y = 200;

        Paint paint = new Paint();
        int color = ContextCompat.getColor(context, R.color.youDied);
        paint.setColor(color);
        float textSize = 150;
        paint.setTextSize(textSize);
        canvas.drawText(text, 100, 100, paint);
    }
}
