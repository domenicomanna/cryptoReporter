/* tslint:disable */
/* eslint-disable */
/**
 * Api
 * No description provided (generated by Openapi Generator https://github.com/openapitools/openapi-generator)
 *
 * The version of the OpenAPI document: 1.0
 * 
 *
 * NOTE: This class is auto generated by OpenAPI Generator (https://openapi-generator.tech).
 * https://openapi-generator.tech
 * Do not edit the class manually.
 */

import { exists, mapValues } from '../runtime';
/**
 * 
 * @export
 * @interface TransactionDTO
 */
export interface TransactionDTO {
    /**
     * 
     * @type {number}
     * @memberof TransactionDTO
     */
    id: number;
    /**
     * 
     * @type {string}
     * @memberof TransactionDTO
     */
    date: string;
    /**
     * 
     * @type {number}
     * @memberof TransactionDTO
     */
    quantityTransacted: number;
    /**
     * 
     * @type {number}
     * @memberof TransactionDTO
     */
    price: number;
    /**
     * 
     * @type {number}
     * @memberof TransactionDTO
     */
    fee: number;
    /**
     * 
     * @type {string}
     * @memberof TransactionDTO
     */
    transactionType: string;
    /**
     * 
     * @type {string}
     * @memberof TransactionDTO
     */
    exchange?: string | null;
    /**
     * 
     * @type {number}
     * @memberof TransactionDTO
     */
    numberOfCoinsSold: number;
    /**
     * 
     * @type {string}
     * @memberof TransactionDTO
     */
    notes?: string | null;
}

/**
 * Check if a given object implements the TransactionDTO interface.
 */
export function instanceOfTransactionDTO(value: object): boolean {
    let isInstance = true;
    isInstance = isInstance && "id" in value;
    isInstance = isInstance && "date" in value;
    isInstance = isInstance && "quantityTransacted" in value;
    isInstance = isInstance && "price" in value;
    isInstance = isInstance && "fee" in value;
    isInstance = isInstance && "transactionType" in value;
    isInstance = isInstance && "numberOfCoinsSold" in value;

    return isInstance;
}

export function TransactionDTOFromJSON(json: any): TransactionDTO {
    return TransactionDTOFromJSONTyped(json, false);
}

export function TransactionDTOFromJSONTyped(json: any, ignoreDiscriminator: boolean): TransactionDTO {
    if ((json === undefined) || (json === null)) {
        return json;
    }
    return {
        
        'id': json['id'],
        'date': json['date'],
        'quantityTransacted': json['quantityTransacted'],
        'price': json['price'],
        'fee': json['fee'],
        'transactionType': json['transactionType'],
        'exchange': !exists(json, 'exchange') ? undefined : json['exchange'],
        'numberOfCoinsSold': json['numberOfCoinsSold'],
        'notes': !exists(json, 'notes') ? undefined : json['notes'],
    };
}

export function TransactionDTOToJSON(value?: TransactionDTO | null): any {
    if (value === undefined) {
        return undefined;
    }
    if (value === null) {
        return null;
    }
    return {
        
        'id': value.id,
        'date': value.date,
        'quantityTransacted': value.quantityTransacted,
        'price': value.price,
        'fee': value.fee,
        'transactionType': value.transactionType,
        'exchange': value.exchange,
        'numberOfCoinsSold': value.numberOfCoinsSold,
        'notes': value.notes,
    };
}
