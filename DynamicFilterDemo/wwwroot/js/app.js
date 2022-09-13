const PropertyTypes =
{
    Number: 1,
    Text: 2,
    Date: 3,
    Boolean: 4,
    Selection: 5
}

const ConditionTypes = {
    Equals: 1,
    GreaterThan: 2,
    LessThan: 3,
    GreaterThanOrEqual: 4,
    LessThanOrEqual: 5,
    IsFalse: 6,
    IsTrue: 7,
    NotEqual: 8,
    StartsWith: 9,
    EndsWith: 10,
    Contains: 11,
    Between: 12
}

class FilterItem {
    constructor(propertyName, propertyType, conditionType, text, value) {
        this.propertyName = propertyName;
        this.propertType = propertyType;
        this.conditionType = conditionType;
        this.text = text;
        this.value = value;
    }

}