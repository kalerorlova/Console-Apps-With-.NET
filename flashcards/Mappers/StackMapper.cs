internal static class StackMapper {

    internal static StackDTO mapToDTO (string stackName, List<Flashcard> cardList) {
        List<FlashcardDTO> myList = new List<FlashcardDTO>();
        foreach (var card in cardList) {
            var cardDTO = FlashcardMapper.mapToDTO(card);
            myList.Add(cardDTO);
        }
        return new StackDTO {
            stackName = stackName,
            flashcards = myList
        };
    }   

}
