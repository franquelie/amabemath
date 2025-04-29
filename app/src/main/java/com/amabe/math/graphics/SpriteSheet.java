package com.amabe.math.graphics;

import android.content.Context;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.graphics.Rect;

import com.amabe.math.R;

public class SpriteSheet {
    private static final int SPRITE_WIDTH_PIXELS = 64;
    private static final int SPRITE_HEIGHT_PIXELS = 64;
    private Bitmap bitmap;

    public SpriteSheet(Context context) {
        BitmapFactory.Options bitmapOptions = new BitmapFactory.Options();
        bitmapOptions.inScaled = false;
        bitmap = BitmapFactory.decodeResource(context.getResources(), R.drawable.sprite_sheet, bitmapOptions);
    }

    public Sprite getPlayerSprite(int x, int y, int width, int height) {
        return new Sprite(this, new Rect(0, 0, 64, 64));
    }

    public Bitmap getBitmap() {
        return bitmap;
    }

    public Sprite getWaterSprite() {
        return getSpriteByIndex(1, 0);
    }

    public Sprite getLavaSprite() {
        return getSpriteByIndex(1, 1);
    }

    public Sprite getGroundSprite() {
        return getSpriteByIndex(1, 2);
    }

    public Sprite getGrassSprite() {
        return getSpriteByIndex(1, 3);
    }

    public Sprite getTreeSprite() {
        return getSpriteByIndex(1, 4);
    }

    public Sprite getWallSprite() {
        return getSpriteByIndex(2, 0);
    }

    public Sprite getWoodDoorSprite() {
        return getSpriteByIndex(2, 2);
    }

    public Sprite getSteelDoorSprite() {
        return getSpriteByIndex(2, 2);
    }

    private Sprite getSpriteByIndex(int idxRow, int idxCol) {
        return new Sprite(this, new Rect(
                idxCol*SPRITE_WIDTH_PIXELS,
                idxRow*SPRITE_HEIGHT_PIXELS,
                (idxCol + 1)*SPRITE_WIDTH_PIXELS,
                (idxRow + 1)*SPRITE_HEIGHT_PIXELS
        ));
    }


}
