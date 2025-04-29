package com.amabe.math.gamemap;

import android.graphics.Canvas;
import android.graphics.Rect;

import com.amabe.math.graphics.SpriteSheet;

abstract class Tile {

    protected final Rect mapLocationRect;

    public Tile(Rect mapLocationRect) {
        this.mapLocationRect = mapLocationRect;
    }

    public enum TileType {
        WATER_TILE,
        LAVA_TILE,
        GROUND_TILE,
        GRASS_TILE,
        TREE_TILE,
        WALL_TILE,
        WOOD_DOOR_TILE,
        STEEL_DOOR_TILE
    }

    public static Tile getTile(int idxTileType, SpriteSheet spriteSheet, Rect mapLocationRect) {

        switch(TileType.values()[idxTileType]) {

            case WATER_TILE:
                return new WaterTile(spriteSheet, mapLocationRect);
            case LAVA_TILE:
                return new LavaTile(spriteSheet, mapLocationRect);
            case GROUND_TILE:
                return new GroundTile(spriteSheet, mapLocationRect);
            case GRASS_TILE:
                return new GrassTile(spriteSheet, mapLocationRect);
            case TREE_TILE:
                return new TreeTile(spriteSheet, mapLocationRect);
            case WALL_TILE:
                return new WallTile(spriteSheet, mapLocationRect);
            case WOOD_DOOR_TILE:
                return new WoodDoorTile(spriteSheet, mapLocationRect);
            case STEEL_DOOR_TILE:
                return new SteelDoorTile(spriteSheet, mapLocationRect);
            default:
                return null;
        }
    }

    public abstract void draw(Canvas canvas);
}






