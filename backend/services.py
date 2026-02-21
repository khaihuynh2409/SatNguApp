import random
from typing import Dict

class WeatherService:
    @staticmethod
    def get_weather_and_tide(lat: float, lon: float) -> Dict:
        # Giả lập gọi API OpenWeatherMap và Tide API
        # Trong thực tế, sẽ dùng thư viện requests với API Key
        
        # Mô phỏng dữ liệu thời tiết
        is_hot = random.choice([True, False])
        temperature = random.randint(30, 36) if is_hot else random.randint(15, 25)
        weather_condition = "nóng" if temperature >= 30 else "lạnh"
        
        # Mô phỏng dữ liệu thủy triều
        tide_condition = random.choice(["nước lớn", "nước ròng"])
        
        return {
            "temperature": temperature,
            "weather_condition": weather_condition,
            "tide_condition": tide_condition,
            "season": "Hạ", # Mock current season
            "month": 2,     # Mock current month
            "time_of_day": "Sáng" # Mock current time
        }

class RecommendationService:
    @staticmethod
    def get_combo_recommendation(db_session, lat: float, lon: float):
        # 1. Lấy thông tin thời tiết & con nước
        context = WeatherService.get_weather_and_tide(lat, lon)
        
        # 2. Query Rules Engine dựa trên context
        # (Ở mức prototype, ta lấy tất cả rule và lọc, hoặc query trực tiếp)
        # Vì db có thể chưa có data mock sẵn, ta sẽ trả ra mock data nếu query không thấy.
        from models import Rule, Fish, Gear, Bait
        
        rules = db_session.query(Rule).filter(
            (Rule.weather_condition == context["weather_condition"]) | (Rule.weather_condition == None),
            (Rule.tide_condition == context["tide_condition"]) | (Rule.tide_condition == None)
        ).all()
        
        if not rules:
            # Fallback mock data nếu DB trống
            return {
                "context": context,
                "recommendation": {
                    "fish_target": "Cá Tra sông (Demo)",
                    "gear": "Cần bạo lực dộ cứng MH, Máy size 4000-6000",
                    "axis_line": "Trục 5.0, Dây nylon siêu bền",
                    "leader": "Thẻo dù 0.4 hoặc Fluoro carbon 0.5",
                    "hook": "Lưỡi săn hàng size 10-12, có ngạnh",
                    "bait": "Cám tanh trộn gan xay"
                },
                "message": "Dữ liệu mẫu cho bản nâng cấp"
            }
            
        # Nếu có rule trong DB, lấy ngẫu nhiên 1 rule hợp lệ
        selected_rule = random.choice(rules)
        fish = db_session.query(Fish).filter(Fish.id == selected_rule.fish_id).first()
        gear = db_session.query(Gear).filter(Gear.id == selected_rule.gear_id).first()
        bait = db_session.query(Bait).filter(Bait.id == selected_rule.bait_id).first()
        
        return {
            "context": context,
            "recommendation": {
                "fish_target": fish.name if fish else "Unknown",
                "gear": f"{gear.name} ({gear.specifications})" if gear else "Unknown",
                "leader": selected_rule.leader_advice if selected_rule.leader_advice else "Tùy chọn",
                "hook": selected_rule.hook_advice if selected_rule.hook_advice else "Tùy chọn",
                "bait": f"{bait.name} ({bait.recipe})" if bait else "Unknown"
            }
        }

    @staticmethod
    def get_fish_catalog(db_session, fish_name: str = None, season: str = None):
        # API phục vụ màn hình Bách khoa Combo
        from models import Rule, Fish, Gear, Bait
        query = db_session.query(Rule).join(Fish)
        
        if fish_name:
            query = query.filter(Fish.name.ilike(f"%{fish_name}%"))
        if season:
            query = query.filter(Rule.season == season)
            
        rules = query.all()
        results = []
        for r in rules:
            f = db_session.query(Fish).filter(Fish.id == r.fish_id).first()
            g = db_session.query(Gear).filter(Gear.id == r.gear_id).first()
            b = db_session.query(Bait).filter(Bait.id == r.bait_id).first()
            results.append({
                "fish": f.name if f else "Unknown",
                "season": r.season,
                "combo": {
                    "gear": g.name if g else "Unknown",
                    "leader": r.leader_advice,
                    "hook": r.hook_advice,
                    "bait": b.name if b else "Unknown"
                }
            })
        return results

class ChatService:
    @staticmethod
    def get_ai_response(message: str, has_image: bool) -> str:
        # Giả lập phản hồi từ LLM (Ví dụ GPT-4 Vision / Gemini)
        message_lower = message.lower()
        
        if has_image:
            return "📸 Tôi đã nhận được ảnh của bác. Nhìn qua thì loại mồi bám móc rất tốt, trạng thái mồi tơi xốp, rất phù hợp để đánh đáy cho cá Chép và Trắm. Tuy nhiên, nếu nước tĩnh bác nên thêm chút hương liệu tanh thơm (ví dụ: tinh mùi dâu hoặc trùn chỉ) để kích thích cá tợp mài nhanh hơn nhé!"
            
        if "mồi" in message_lower or "cám" in message_lower:
            return "🎣 Về mồi câu đài, bác cứ nhớ nguyên tắc: 'Mùa lạnh đánh tanh, mùa nóng đánh thơm/chua'. Hiện tại đang mùa Nóng, bác nên ưu tiên các loại mồi có vị ngũ cốc lên men, vị trái cây (ổi, dâu) hoặc mồi bắp ủ chua nhé."
            
        if "thẻo" in message_lower or "trục" in message_lower or "phao" in message_lower:
            return "📏 Với hệ Câu Đài, trục và thẻo rất quan trọng. Trục nylon 2.0 và thẻo 1.2 là thông số tiêu chuẩn cho cá từ 2-5kg. Khi cân phao, bác cứ cân 5 câu 3 (hoặc 7 câu 3) để tín hiệu báo sập phao chuẩn nhất nhé."
            
        if "cá chép" in message_lower:
            return "🐟 Cá Chép rất khôn và ăn nhát. Bác nên xả ổ thật êm, mồi vê tròn mềm để cá hút dễ dàng. Tránh tiếng động mạnh và dùng phao ngọn nhỏ, ăn chì ít (tầm 1.5 - 2.5g) để thấy rõ nhịp tăm lên."
            
        return "🤖 Chào bác! Em là Trợ Lý AI chuyên Câu Đài của Sát Ngư. Bác cần tư vấn về mồi, trục thẻo, hay muốn em đánh giá địa hình/mồi qua ảnh chụp thì cứ nhắn em nhé!"
