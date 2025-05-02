-- Seed dữ liệu cho bảng Users
INSERT INTO dbo.Users (UserName, Password, FullName, Email, PhoneNumber, Address, Gender, DateOfBirth, UserStatus)
VALUES
    ('thedung', 'e10adc3949ba59abbe56e057f20f883e', N'Thế Dũng', 'thedung@gmail.com', '0901234567', N'Thái Bình', N'Nam', '2003-04-30', 0),
    ('hoanglan', 'e10adc3949ba59abbe56e057f20f883e', N'Hoàng Lan', 'hoanglan@gmail.com', '0907654321', N'Hồ Chí Minh', N'Nữ', '1995-05-15', 0),
    ('trangvann', 'e10adc3949ba59abbe56e057f20f883e', N'Trần Văn Nam', 'trangvann@gmail.com', '0912345678', N'Đà Nẵng', N'Nam', '1988-03-20', 0),
    ('lethu', 'e10adc3949ba59abbe56e057f20f883e', N'Lê Thu', 'lethu@gmail.com', '0987654321', N'Hải Phòng', N'Nữ', '1992-07-12', 0);

	-- Seed dữ liệu cho bảng Posts
INSERT INTO dbo.Posts (Content, CreateAt, UpdateAt, UserId, PostType, PostStatus, PostVisibility, IsDeleted)
VALUES
    (N'Hôm nay là một ngày đẹp trời!', '2025-05-02 10:00:00', '2025-05-02 10:00:00', 1, 2, 0, 2, 0),
    (N'Chúc mừng năm mới mọi người!', '2025-05-02 11:00:00', '2025-05-02 11:00:00', 2, 2, 0, 2, 0),
    (N'Mình vừa mua một chiếc xe mới, siêu đẹp!', '2025-05-02 12:00:00', '2025-05-02 12:00:00', 3, 2, 0, 2, 0),
    (N'Đã đi du lịch và có rất nhiều kỷ niệm!', '2025-05-02 13:00:00', '2025-05-02 13:00:00', 4, 2, 0, 2, 0);

	-- Seed dữ liệu cho bảng Comments
INSERT INTO dbo.Comments (Content, CreateAt, UpdateAt, UserId, PostId, IsLink, IsRevoked)
VALUES
    (N'Bài viết tuyệt vời!', '2025-05-02 10:30:00', '2025-05-02 10:30:00', 1, 1, 0, 0),
    (N'Mình cũng rất thích xe đó!', '2025-05-02 12:30:00', '2025-05-02 12:30:00', 4, 1, 0, 0),
    (N'Chúc bạn năm mới vui vẻ!', '2025-05-02 11:30:00', '2025-05-02 11:30:00', 3, 1, 0, 0);

		-- Seed dữ liệu cho bảng ChatRooms
INSERT INTO dbo.ChatRooms (LeaderId, UserIds, Name, IsGroupChat, IsDelete, CanAddMember, CanSendMessage)
VALUES
    (1, '[1,2,3,4]', N'Nhóm bạn thân', 1, 0, 1, 1),
    (2, '[1,4]', null, 0, 0, 1, 1),
    (3, '[1,2]', null, 0, 0, 1, 1);

	-- Seed dữ liệu cho bảng Messages
INSERT INTO dbo.Messages (Content, CreateAt, UserId, ChatRoomId, IsLink, IsRevoked)
VALUES
    (N'Chào mọi người, ai đang online không?', '2025-05-02 14:00:00', 1, 1, 0, 0),
    (N'Mình ở đây nè, có chuyện gì không?', '2025-05-02 14:05:00', 2, 1, 0, 0),
    (N'Chúc mừng năm mới tất cả!', '2025-05-02 14:10:00', 3, 2, 0, 0),
    (N'Xe của bạn đẹp quá, khi nào chạy thử?', '2025-05-02 14:15:00', 1, 3, 0, 0);



	
INSERT INTO dbo.Notifications (UserId, Type, ReferenceId, Content, IsRead, CreateAt)
VALUES
    (1, 0, 1, N'', 0, '2025-05-02 10:30:00'),
    (1, 0, 1, N'', 0, '2025-05-02 14:00:00'),
    (1, 0, 2, N'', 0, '2025-05-02 13:00:00'),
	(1, 0, 3, N'', 1, '2025-05-02 13:00:00');


	-- Seed dữ liệu cho bảng User_ChatRoom
INSERT INTO dbo.User_ChatRoom (UserId, ChatRoomId, IsLeft)
VALUES
    (1, 1, 0), 
    (2, 1, 0), 
    (3, 1, 0), 
    (4, 1, 0), 
	
    (1, 2, 0),  
    (4, 2, 0),  

    (1, 3, 0),  
    (2, 3, 0);  

