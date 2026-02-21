from sqlalchemy import Boolean, Column, ForeignKey, Integer, String, Float, Text
from sqlalchemy.orm import relationship
from database import Base

class User(Base):
    __tablename__ = "users"
    id = Column(Integer, primary_key=True, index=True)
    username = Column(String, unique=True, index=True)
    hashed_password = Column(String)
    is_active = Column(Boolean, default=True)

class Fish(Base):
    __tablename__ = "fishes"
    id = Column(Integer, primary_key=True, index=True)
    name = Column(String, unique=True, index=True)
    description = Column(Text, nullable=True)
    habitat_type = Column(String) # e.g., "sông", "hồ", "biển"
    water_layer = Column(String) # e.g., "đáy", "lửng", "mặt"

class Gear(Base):
    __tablename__ = "gears"
    id = Column(Integer, primary_key=True, index=True)
    name = Column(String, index=True)
    gear_type = Column(String) # e.g., "rod", "reel", "line", "hook"
    specifications = Column(String) # e.g., "size 4000", "size 5.0"
    
class Bait(Base):
    __tablename__ = "baits"
    id = Column(Integer, primary_key=True, index=True)
    name = Column(String, index=True)
    bait_type = Column(String) # e.g., "công nghiệp", "tự nhiên", "cám"
    recipe = Column(Text, nullable=True) # Mixing instructions or details

class Rule(Base):
    __tablename__ = "rules"
    id = Column(Integer, primary_key=True, index=True)
    fish_id = Column(Integer, ForeignKey("fishes.id"))
    gear_id = Column(Integer, ForeignKey("gears.id"))
    bait_id = Column(Integer, ForeignKey("baits.id"))
    weather_condition = Column(String, nullable=True) # e.g., "nóng", "lạnh"
    tide_condition = Column(String, nullable=True) # e.g., "nước lớn", "nước ròng"
    season = Column(String, nullable=True) # e.g., "Xuân", "Hạ", "Thu", "Đông"
    month = Column(Integer, nullable=True)
    time_of_day = Column(String, nullable=True) # e.g., "Sáng", "Trưa", "Chiều", "Tối"
    hook_advice = Column(String, nullable=True) # e.g., "Size 10, có ngạnh"
    leader_advice = Column(String, nullable=True) # e.g., "Fluoro 0.3mm"
