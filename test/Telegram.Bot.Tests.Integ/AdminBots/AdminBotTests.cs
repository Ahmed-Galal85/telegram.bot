﻿using System;
using System.IO;
using System.Threading.Tasks;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Tests.Integ.Common;
using Telegram.Bot.Types;
using Xunit;

namespace Telegram.Bot.Tests.Integ.AdminBots
{
    [Collection(CommonConstants.TestCollections.AdminBots)]
    [TestCaseOrderer(CommonConstants.TestCaseOrderer, CommonConstants.AssemblyName)]
    public class AdminBotTests : IClassFixture<AdminBotTestFixture>
    {
        private readonly AdminBotTestFixture _classFixture;

        private readonly TestsFixture _fixture;

        private ITelegramBotClient BotClient => _fixture.BotClient;

        public AdminBotTests(AdminBotTestFixture classFixture)
        {
            _classFixture = classFixture;
            _fixture = _classFixture.TestsFixture;
        }

        #region 1. Changing Chat Title

        [Fact(DisplayName = FactTitles.ShouldSetChatTitle)]
        [Trait(CommonConstants.MethodTraitName, CommonConstants.TelegramBotApiMethods.SetChatTitle)]
        [ExecutionOrder(1)]
        public async Task Should_Set_Chat_Title()
        {
            await _fixture.SendTestCaseNotificationAsync(FactTitles.ShouldSetChatTitle);

            bool result = await BotClient.SetChatTitleAsync(_fixture.SuperGroupChatId, _classFixture.ChatTitle);

            Assert.True(result);
        }

        #endregion

        #region 2. Changing Chat Description

        [Fact(DisplayName = FactTitles.ShouldSetChatDescription)]
        [Trait(CommonConstants.MethodTraitName, CommonConstants.TelegramBotApiMethods.SetChatDescription)]
        [ExecutionOrder(2.1)]
        public async Task Should_Set_Chat_Description()
        {
            await _fixture.SendTestCaseNotificationAsync(FactTitles.ShouldSetChatDescription);

            bool result = await BotClient.SetChatDescriptionAsync(_fixture.SuperGroupChatId,
                "Test Chat Description");

            Assert.True(result);
        }

        [Fact(DisplayName = FactTitles.ShouldDeleteChatDescription)]
        [Trait(CommonConstants.MethodTraitName, CommonConstants.TelegramBotApiMethods.SetChatDescription)]
        [ExecutionOrder(2.2)]
        public async Task Should_Delete_Chat_Description()
        {
            await _fixture.SendTestCaseNotificationAsync(FactTitles.ShouldDeleteChatDescription);

            bool result = await BotClient.SetChatDescriptionAsync(_fixture.SuperGroupChatId);

            Assert.True(result);
        }

        #endregion

        #region 3. Pinning Chat Message

        [Fact(DisplayName = FactTitles.ShouldPinMessage)]
        [Trait(CommonConstants.MethodTraitName, CommonConstants.TelegramBotApiMethods.PinChatMessage)]
        [ExecutionOrder(3.1)]
        public async Task Should_Pin_Message()
        {
            Message msg = await _fixture.SendTestCaseNotificationAsync(FactTitles.ShouldPinMessage);

            bool result = await BotClient.PinChatMessageAsync(_fixture.SuperGroupChatId, msg.MessageId);

            Assert.True(result);
            _classFixture.PinnedMessage = msg;
        }

        [Fact(DisplayName = FactTitles.ShouldGetChatPinnedMessage)]
        [Trait(CommonConstants.MethodTraitName, CommonConstants.TelegramBotApiMethods.GetChat)]
        [ExecutionOrder(3.2)]
        public async Task Should_Get_Chat_Pinned_Message()
        {
            await _fixture.SendTestCaseNotificationAsync(FactTitles.ShouldGetChatPinnedMessage);

            Message pinnedMsg = _classFixture.PinnedMessage;
            if (pinnedMsg is null)
            {
                throw new NullReferenceException("Pinned message should have been set in the preceding test");
            }

            Chat chat = await BotClient.GetChatAsync(_fixture.SuperGroupChatId);

            Assert.Equal(pinnedMsg.MessageId, chat.PinnedMessage.MessageId);
            Assert.Equal(pinnedMsg.Text, chat.PinnedMessage.Text);
        }

        [Fact(DisplayName = FactTitles.ShouldUnpinMessage)]
        [Trait(CommonConstants.MethodTraitName, CommonConstants.TelegramBotApiMethods.UnpinChatMessage)]
        [ExecutionOrder(3.3)]
        public async Task Should_Unpin_Message()
        {
            await _fixture.SendTestCaseNotificationAsync(FactTitles.ShouldUnpinMessage);

            bool result = await BotClient.UnpinChatMessageAsync(_fixture.SuperGroupChatId);

            Assert.True(result);
        }

        [Fact(DisplayName = FactTitles.ShouldGetChatWithNoPinnedMessage)]
        [Trait(CommonConstants.MethodTraitName, CommonConstants.TelegramBotApiMethods.GetChat)]
        [ExecutionOrder(3.4)]
        public async Task Should_Get_Chat_With_No_Pinned_Message()
        {
            await _fixture.SendTestCaseNotificationAsync(FactTitles.ShouldGetChatWithNoPinnedMessage);

            Chat chat = await BotClient.GetChatAsync(_fixture.SuperGroupChatId);

            Assert.Null(chat.PinnedMessage);
        }

        #endregion

        #region 4. Changing Chat Photo

        [Fact(DisplayName = FactTitles.ShouldSetChatPhoto)]
        [Trait(CommonConstants.MethodTraitName, CommonConstants.TelegramBotApiMethods.SetChatPhoto)]
        [ExecutionOrder(4.1)]
        public async Task Should_Set_Chat_Photo()
        {
            await _fixture.SendTestCaseNotificationAsync(FactTitles.ShouldSetChatPhoto);
            bool result;

            using (var stream = new FileStream("Files/Photo/t_logo.png", FileMode.Open))
            {
                result = await BotClient.SetChatPhotoAsync(_fixture.SuperGroupChatId,
                    new FileToSend("photo.png", stream));
            }

            Assert.True(result);
        }

        [Fact(DisplayName = FactTitles.ShouldDeleteChatPhoto)]
        [Trait(CommonConstants.MethodTraitName, CommonConstants.TelegramBotApiMethods.DeleteChatPhoto)]
        [ExecutionOrder(4.2)]
        public async Task Should_Delete_Chat_Photo()
        {
            await _fixture.SendTestCaseNotificationAsync(FactTitles.ShouldDeleteChatPhoto);

            bool result = await BotClient.DeleteChatPhotoAsync(_fixture.SuperGroupChatId);

            Assert.True(result);
        }

        [Fact(DisplayName = FactTitles.ShouldThrowOnDeletingChatDeletedPhoto)]
        [Trait(CommonConstants.MethodTraitName, CommonConstants.TelegramBotApiMethods.DeleteChatPhoto)]
        [ExecutionOrder(4.3)]
        public async Task Should_Throw_On_Deleting_Chat_Deleted_Photo()
        {
            await _fixture.SendTestCaseNotificationAsync(FactTitles.ShouldThrowOnDeletingChatDeletedPhoto);

            Exception e = await Assert.ThrowsAnyAsync<Exception>(() =>
                BotClient.DeleteChatPhotoAsync(_fixture.SuperGroupChatId));

            Assert.IsType<ApiRequestException>(e);
            Assert.Equal("Bad Request: CHAT_NOT_MODIFIED", e.Message);
        }

        #endregion

        #region 5. Chat Sticker Set

        [Fact(DisplayName = FactTitles.ShouldThrowOnSettingChatStickerSet)]
        [Trait(CommonConstants.MethodTraitName, CommonConstants.TelegramBotApiMethods.SetChatStickerSet)]
        [ExecutionOrder(5)]
        public async Task Should_Throw_On_Setting_Chat_Sticker_Set()
        {
            await _fixture.SendTestCaseNotificationAsync(FactTitles.ShouldThrowOnSettingChatStickerSet);

            const string setName = "EvilMinds";

            Exception e = await Assert.ThrowsAnyAsync<Exception>(() =>
                _fixture.BotClient.SetChatStickerSetAsync(_fixture.SuperGroupChatId, setName)
            );

            Assert.IsType<ApiRequestException>(e);

            var requestException = (ApiRequestException) e;
            Assert.Equal(400, requestException.ErrorCode);
            Assert.Equal("Bad Request: can't set channel sticker set", requestException.Message);
        }

        #endregion

        private static class FactTitles
        {
            public const string ShouldSetChatTitle = "Should set chat title";

            public const string ShouldSetChatDescription = "Should set chat description";

            public const string ShouldDeleteChatDescription = "Should delete chat description";

            public const string ShouldPinMessage = "Should pin chat message";

            public const string ShouldGetChatPinnedMessage = "Should get chat's pinned message";

            public const string ShouldUnpinMessage = "Should unpin chat message";

            public const string ShouldGetChatWithNoPinnedMessage = "Should get the chat info without a pinned message";

            public const string ShouldSetChatPhoto = "Should set chat photo";

            public const string ShouldDeleteChatPhoto = "Should delete chat photo";

            public const string ShouldThrowOnDeletingChatDeletedPhoto =
                "Should throw exception in deleting chat photo with no photo currently set";

            public const string ShouldThrowOnSettingChatStickerSet =
                "Should throw exception when trying to set sticker set for a chat with less than 100 members";
        }
    }
}