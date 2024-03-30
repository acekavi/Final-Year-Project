import mongoose, { Schema, Document } from 'mongoose';

// Define an interface for the User document
export interface IUser extends Document {
    email: string;
    name: string;
    age: number;
    password: string;
    created_at: Date;
    achievements: string[];
    badges: string[];
    level: number;
}

// Define the User schema
const userSchema: Schema = new mongoose.Schema({
    _id: mongoose.Schema.Types.ObjectId,
    email: {
        type: String,
        required: true,
        match: /[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?/,
    },
    name: { type: String, required: true },
    age: { type: Number, required: true },
    password: { type: String, required: true },
    created_at: { type: Date, default: Date.now },
    achievements: { type: [String], required: false, default: [] },
    badges: { type: [String], required: false, default: [] },
    level: { type: Number, required: false, default: 1 },
});

// Create and export the model
const User = mongoose.model<IUser>('User', userSchema);
export default User;
