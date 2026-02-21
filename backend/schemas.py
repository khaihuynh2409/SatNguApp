from pydantic import BaseModel
from typing import Optional

class FishBase(BaseModel):
    name: str
    description: Optional[str] = None
    habitat_type: str
    water_layer: str

class FishCreate(FishBase):
    pass

class Fish(FishBase):
    id: int

    class Config:
        from_attributes = True

class GearBase(BaseModel):
    name: str
    gear_type: str
    specifications: str

class GearCreate(GearBase):
    pass

class Gear(GearBase):
    id: int

    class Config:
        from_attributes = True

class BaitBase(BaseModel):
    name: str
    bait_type: str
    recipe: Optional[str] = None

class BaitCreate(BaitBase):
    pass

class Bait(BaitBase):
    id: int

    class Config:
        from_attributes = True

class RuleBase(BaseModel):
    fish_id: int
    gear_id: int
    bait_id: int
    weather_condition: Optional[str] = None
    tide_condition: Optional[str] = None
    season: Optional[str] = None
    month: Optional[int] = None
    time_of_day: Optional[str] = None
    hook_advice: Optional[str] = None
    leader_advice: Optional[str] = None

class RuleCreate(RuleBase):
    pass

class Rule(RuleBase):
    id: int

    class Config:
        from_attributes = True

class ChatRequest(BaseModel):
    message: str
    image_base64: Optional[str] = None

class ChatResponse(BaseModel):
    reply: str
