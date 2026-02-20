from fastapi import FastAPI, Depends, HTTPException
from sqlalchemy.orm import Session
from typing import List

import models, schemas
from database import engine, get_db

models.Base.metadata.create_all(bind=engine)

app = FastAPI(title="Sát Ngư App API", description="Backend API cho ứng dụng câu cá", version="1.0.0")

@app.get("/")
def read_root():
    return {"message": "Welcome to Sát Ngư App API"}

@app.get("/health")
def health_check():
    return {"status": "ok"}

# --- Fish CRUD ---
@app.post("/fishes/", response_model=schemas.Fish)
def create_fish(fish: schemas.FishCreate, db: Session = Depends(get_db)):
    db_fish = models.Fish(**fish.model_dump())
    db.add(db_fish)
    db.commit()
    db.refresh(db_fish)
    return db_fish

@app.get("/fishes/", response_model=List[schemas.Fish])
def read_fishes(skip: int = 0, limit: int = 100, db: Session = Depends(get_db)):
    fishes = db.query(models.Fish).offset(skip).limit(limit).all()
    return fishes

# --- Gear CRUD ---
@app.post("/gears/", response_model=schemas.Gear)
def create_gear(gear: schemas.GearCreate, db: Session = Depends(get_db)):
    db_gear = models.Gear(**gear.model_dump())
    db.add(db_gear)
    db.commit()
    db.refresh(db_gear)
    return db_gear

@app.get("/gears/", response_model=List[schemas.Gear])
def read_gears(skip: int = 0, limit: int = 100, db: Session = Depends(get_db)):
    gears = db.query(models.Gear).offset(skip).limit(limit).all()
    return gears

# --- Bait CRUD ---
@app.post("/baits/", response_model=schemas.Bait)
def create_bait(bait: schemas.BaitCreate, db: Session = Depends(get_db)):
    db_bait = models.Bait(**bait.model_dump())
    db.add(db_bait)
    db.commit()
    db.refresh(db_bait)
    return db_bait

@app.get("/baits/", response_model=List[schemas.Bait])
def read_baits(skip: int = 0, limit: int = 100, db: Session = Depends(get_db)):
    baits = db.query(models.Bait).offset(skip).limit(limit).all()
    return baits

# --- Rule CRUD ---
@app.post("/rules/", response_model=schemas.Rule)
def create_rule(rule: schemas.RuleCreate, db: Session = Depends(get_db)):
    db_rule = models.Rule(**rule.model_dump())
    db.add(db_rule)
    db.commit()
    db.refresh(db_rule)
    return db_rule

@app.get("/rules/", response_model=List[schemas.Rule])
def read_rules(skip: int = 0, limit: int = 100, db: Session = Depends(get_db)):
    rules = db.query(models.Rule).offset(skip).limit(limit).all()
    return rules

# --- Services ---
from services import RecommendationService
import math
import random

@app.get("/recommendation/")
def get_recommendation(lat: float, lon: float, db: Session = Depends(get_db)):
    return RecommendationService.get_combo_recommendation(db, lat, lon)

@app.get("/biorhythm/")
def get_biorhythm(fish_id: int):
    # Mock data for Biorhythm / Đồng hồ sinh học (Biểu đồ cắn câu)
    hours = list(range(0, 24))
    activity_levels = [round(abs(math.sin(h/12 * math.pi)) * 100, 2) for h in hours] # Mock wave
    # Add random noise
    activity_levels = [max(0, min(100, val + random.randint(-10, 10))) for val in activity_levels]
    
    return {
        "fish_id": fish_id,
        "hourly_activity": dict(zip(hours, activity_levels)),
        "advice": "Mùa lạnh cá nằm đáy ấm, cần câu mồi protein cao. Mùa hè cá đi lửng thích mồi thơm chua ngọt."
    }
