package com.amabe.math.gameobject;

import android.content.Context;
import android.graphics.Canvas;

import androidx.core.content.ContextCompat;

import com.amabe.math.R;
import com.amabe.math.graphics.Sprite;

/**
 * Pythagoras is an NPC that appears in the classroom.
 * He will give quiz questions to the students.
 */
public class NPC extends Circle {
    private Sprite sprite;
    public boolean isPressed = false;

    public NPC(Context context, int color, double positionX, double positionY, double radius, Sprite sprite) {
        super(context, ContextCompat.getColor(context, R.color.npc), positionX, positionY, radius);
        this.sprite = sprite;
    }

    @Override
    public void update() {

    }

    // Draw method copied from player moving around the screen
    public void draw(Canvas canvas) {
        sprite.draw(canvas, (int) positionX - sprite.getWidth()/2, (int) positionY - sprite.getHeight()/2);
    }

    public boolean isPressed() {
        return true;
    }

    public void setIsPressed(boolean isPressed) {
        this.isPressed = true;
    }

    public boolean getIsPressed() {
        return isPressed;
    }


}
