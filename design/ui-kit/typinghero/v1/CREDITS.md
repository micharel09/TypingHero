# Credits và provenance

- Artwork nền và foreground: mới tạo bằng công cụ ImageGen tích hợp trong phiên 19/10/2026. Prompt lưu tại [source/imagegen-prompts.json](source/imagegen-prompts.json). Hai PNG `*-original.png` giữ đầu ra nguyên bản; bản 640×360 được nearest-neighbor và giới hạn palette/alpha trong `source/build.py`.
- Logo, icon, controls, frames, HUD shape và foreground vector alternative: sáng tác bằng pixel/vector trong `source/build.py`, kèm SVG tương ứng. Không sử dụng frame hoặc icon của bên thứ ba.
- Font chức năng: **VT323**, Peter Hull, SIL Open Font License. Sao chép từ font đang có trong repo; [license đầy đủ](fonts/VT323-OFL.txt).
- Nhân vật hero: lấy đúng frame đầu trong `Assets/Art/male_hero/individual_sheets/male_hero-idle.png`, GUID nguồn `8f5006e6b1790f9449b7ba6708ae9e38`. Không chỉnh pose, vũ khí, palette hay animation.
- Skeleton trong mockup: frame đầu từ `Assets/Art/EndlessSkeletons/sword/Idle.png`, import rect 96×64; chỉ flip theo hướng đối đầu trong mockup. Giữ nguyên asset gốc trong game.
- Các sprite nhân vật thuộc bộ asset hiện có của project. Không tìm thấy file license/credits trong hai thư mục sprite khi kiểm tra source; gói này không bổ sung tuyên bố cấp phép mới cho chúng. Bản crop chỉ dùng trình bày thiết kế cùng project hiện tại.
- Tham khảo thị giác do user chọn: [Shogun Showdown / UDN](https://game.udn.com/game/story/122088/8224224), [Type Knight / chaikadev](https://chaikadev.itch.io/type-knight). Không đưa ảnh hoặc tài nguyên của hai game vào gói.

PNG/SVG do script tạo và artwork AI được phân biệt rõ; không gọi bản SVG alternative là vector hóa ảnh AI.
