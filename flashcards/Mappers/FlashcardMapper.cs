
internal static class FlashcardMapper {
    internal static FlashcardDTO mapToDTO (Flashcard card) {
        return new FlashcardDTO {
            question = card.question,
            answer = card.answer
        };
    }
}
