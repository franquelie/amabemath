package com.amabe.math.gamepanel;

import android.content.Context;
import android.graphics.Canvas;
import android.graphics.Paint;

import androidx.core.content.ContextCompat;

import com.amabe.math.R;
import com.amabe.math.gameobject.NPC;

public class NewPanel {
    private Context context;
    private String text;

    public NewPanel(Context context) {
        this.context = context;
    }

    public void draw(Canvas canvas, NPC pythagoras) {

        text = String.format("IsPressed() = %s", pythagoras.isPressed);
        float x = 800;
        float y = 200;

        Paint paint = new Paint();
        int color = ContextCompat.getColor(context, R.color.youDied);
        paint.setColor(color);
        float textSize = 150;
        paint.setTextSize(textSize);
        canvas.drawText(text, 100, 100, paint);
    }

    public void update(Context canvas, NPC pythagoras) {
        text = String.format("IsPressed() = %s", pythagoras.isPressed);
    }
}
