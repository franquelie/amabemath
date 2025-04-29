package com.amabe.math.gamemap;

import android.graphics.Canvas;
import android.graphics.Rect;

import com.amabe.math.graphics.Sprite;
import com.amabe.math.graphics.SpriteSheet;

public class SteelDoorTile extends Tile{
    private final Sprite sprite;

    public SteelDoorTile(SpriteSheet spriteSheet, Rect mapLocationRect) {
        super(mapLocationRect);
        sprite = spriteSheet.getSteelDoorSprite();
    }

    @Override
    public void draw(Canvas canvas) {
        sprite.draw(canvas, mapLocationRect.left, mapLocationRect.top);
    }
}